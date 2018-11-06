import React, { PureComponent } from 'react';
import { findDOMNode } from 'react-dom';
import moment from 'moment';
import { connect } from 'dva';
import {
  List,
  Card,
  Row,
  Col,
  Radio,
  Input,
  Progress,
  Button,
  Icon,
  Dropdown,
  Menu,
  Avatar,
  Modal,
  Form,
  DatePicker,
  Select,
} from 'antd';

import PageHeaderWrapper from '@/components/PageHeaderWrapper';
import Result from '@/components/Result';
import Ellipsis from '@/components/Ellipsis';
import styles from './BasicList.less';
import cardStyles from './CardList.less';

const FormItem = Form.Item;
const RadioButton = Radio.Button;
const RadioGroup = Radio.Group;
const SelectOption = Select.Option;
const { Search, TextArea } = Input;

@connect(({ user, alert, loading }) => ({
  alerts: alert.alerts,
  currentUser: user.currentUser,
  loading: loading.models.alerts,
}))
@Form.create()
class BasicList extends PureComponent {
  state = { visible: false, done: false,alertBackground: '#F44E47'};

  formLayout = {
    labelCol: { span: 7 },
    wrapperCol: { span: 13 },
  };

  componentDidMount() {
    const { dispatch } = this.props;
    dispatch({
      type: 'alert/fetch',
    });

    this.timer = setInterval(()=>{
      if(this.state.alertBackground == '#F44E47') {
        this.setState({
          alertBackground:'#fff'
        })
      }else {
        this.setState({
          alertBackground:'#F44E47'
        })
      }
    },1000)
  }

  componentWillUnmount() {
    clearInterval(this.timer);
  }

  showModal = () => {
    this.setState({
      visible: true,
      current: undefined,
    });
  };

  showEditModal = item => {
    this.setState({
      visible: true,
      current: item,
    });
  };

  handleDone = () => {
    setTimeout(() => this.addBtn.blur(), 0);
    this.setState({
      done: false,
      visible: false,
    });
  };

  handleCancel = () => {
    // setTimeout(() => this.addBtn.blur(), 0);
    this.setState({
      visible: false,
    });
  };

  handleSubmit = e => {
    e.preventDefault();
    const { dispatch, form } = this.props;
    const { current } = this.state;
    const id = current ? current.id : '';

    setTimeout(() => this.addBtn.blur(), 0);
    form.validateFields((err, fieldsValue) => {
      if (err) return;
      this.setState({
        done: true,
      });
      dispatch({
        type: 'list/submit',
        payload: { id, ...fieldsValue },
      });
    });
  };

  deleteItem = id => {
    const { dispatch } = this.props;
    dispatch({
      type: 'list/submit',
      payload: { id },
    });
  };

  renderItem = item => {
    const isNewAlert = item.status == 0;
    let toggleBackground = isNewAlert?{ backgroundColor: this.state.alertBackground }:{};
    return (<List.Item key={item.id}>
        <Card hoverable className={cardStyles.card} actions={[<a>响应</a>]} style={toggleBackground}>
          <Card.Meta
            avatar={<Avatar style={{ color: '#fff', backgroundColor:  isNewAlert ? '#F44E47' : 'green' }}
                            icon={isNewAlert ? 'alert' : 'phone'} shape="square" size="large"/>}
            title={<a>{item.title}</a>}
            description={
              <Ellipsis className={cardStyles.item} lines={3}>
                {item.description}<br/>{moment(item.alertTime).format("YYYY年MM月DD日 HH:mm:ss")}
              </Ellipsis>
            }
          />
        </Card>
      </List.Item>);
  };

  render() {
    const { alerts, loading, form: { getFieldDecorator } } = this.props;
    const newAlerts = alerts.filter(a => a.status == 0);
    const { visible, done, current = {} } = this.state;

    const editAndDelete = (key, currentItem) => {
      if (key === 'edit') this.showEditModal(currentItem);
      else if (key === 'delete') {
        Modal.confirm({
          title: '删除任务',
          content: '确定删除该任务吗？',
          okText: '确认',
          cancelText: '取消',
          onOk: () => this.deleteItem(currentItem.id),
        });
      }
    };

    const modalFooter = done
      ? { footer: null, onCancel: this.handleDone }
      : { okText: '保存', onOk: this.handleSubmit, onCancel: this.handleCancel };

    const Info = ({ title, value, bordered, style }) => (
      <div className={styles.headerInfo}>
        <span>{title}</span>
        <p style={style}>{value}</p>
        {bordered && <em/>}
      </div>
    );

    const extraContent = (
      <div className={styles.extraContent}>
        <RadioGroup defaultValue="all">
          <RadioButton value="all">全部</RadioButton>
          <RadioButton value="progress">未处理</RadioButton>
          <RadioButton value="waiting">已处理</RadioButton>
        </RadioGroup>
        <Search className={styles.extraContentSearch} placeholder="请输入床位号" onSearch={() => ({})}/>
      </div>
    );

    const paginationProps = {
      showSizeChanger: true,
      showQuickJumper: true,
      pageSize: 20,
      total: this.props.alerts.length,
    };

    const ListContent = ({ data: { respondUserName, alertTime } }) => (
      <div className={styles.listContent}>
        <div className={styles.listContentItem} style={{ width: 180 }}>
          <span>处理人</span>
          <p>{respondUserName}</p>
        </div>
        <div className={styles.listContentItem}>
          <span>警报时间</span>
          <p>{moment(alertTime).format('YYYY-MM-DD HH:mm:ss')}</p>
        </div>
        {/*<div className={styles.listContentItem}>*/}
        {/*<Progress type={'circle'} percent={100} status={'success'} strokeWidth={6} width={40} />*/}
        {/*</div>*/}
      </div>
    );

    const MoreBtn = props => (
      <Dropdown
        overlay={
          <Menu onClick={({ key }) => editAndDelete(key, props.current)}>
            <Menu.Item key="edit">编辑</Menu.Item>
            <Menu.Item key="delete">删除</Menu.Item>
          </Menu>
        }
      >
        <a>
          更多 <Icon type="down"/>
        </a>
      </Dropdown>
    );

    const getModalContent = () => {
      if (done) {
        return (
          <Result
            type="success"
            title="操作成功"
            description="一系列的信息描述，很短同样也可以带标点。"
            actions={
              <Button type="primary" onClick={this.handleDone}>
                知道了
              </Button>
            }
            className={styles.formResult}
          />
        );
      }
      return (
        <Form onSubmit={this.handleSubmit}>
          <FormItem label="任务名称" {...this.formLayout}>
            {getFieldDecorator('title', {
              rules: [{ required: true, message: '请输入任务名称' }],
              initialValue: current.title,
            })(<Input placeholder="请输入"/>)}
          </FormItem>
          <FormItem label="开始时间" {...this.formLayout}>
            {getFieldDecorator('createdAt', {
              rules: [{ required: true, message: '请选择开始时间' }],
              initialValue: current.createdAt ? moment(current.createdAt) : null,
            })(
              <DatePicker
                showTime
                placeholder="请选择"
                format="YYYY-MM-DD HH:mm:ss"
                style={{ width: '100%' }}
              />,
            )}
          </FormItem>
          <FormItem label="任务负责人" {...this.formLayout}>
            {getFieldDecorator('owner', {
              rules: [{ required: true, message: '请选择任务负责人' }],
              initialValue: current.owner,
            })(
              <Select placeholder="请选择">
                <SelectOption value="付晓晓">付晓晓</SelectOption>
                <SelectOption value="周毛毛">周毛毛</SelectOption>
              </Select>,
            )}
          </FormItem>
          <FormItem {...this.formLayout} label="产品描述">
            {getFieldDecorator('subDescription', {
              rules: [{ message: '请输入至少五个字符的产品描述！', min: 5 }],
              initialValue: current.subDescription,
            })(<TextArea rows={4} placeholder="请输入至少五个字符"/>)}
          </FormItem>
        </Form>
      );
    };

    const getActions = (item) => {
      if (item.status == 0) {
        return [<a
          onClick={e => {
            e.preventDefault();
            this.showEditModal(item);
          }}
        >
          待处理
        </a>];
      }

      return [<p style={{ color: 'green' }}>
        已处理
      </p>];
    };

    return (
      <PageHeaderWrapper>
        <div className={styles.standardList}>
          <Card bordered={false}>
            <Row>
              <Col sm={8} xs={24}>
                <Info title="未处理警报" value={newAlerts.length} style={{ color: 'red' }} bordered/>
              </Col>
              <Col sm={8} xs={24}>
                <Info title="已处理警报" value={alerts.length - newAlerts.length} bordered/>
              </Col>
              <Col sm={8} xs={24}>
                <Info title="总警报数" value={alerts.length}/>
              </Col>
            </Row>
          </Card>

          <Card
            className={styles.listCard}
            bordered={false}
            title="警报列表"
            style={{ marginTop: 24 }}
            bodyStyle={{ padding: '0 32px 40px 32px' }}
            extra={extraContent}
          >
            <div className={cardStyles.cardList}>
              <List
                rowKey="id"
                loading={loading}
                grid={{ gutter: 24, lg: 4, md: 2, sm: 1, xs: 1 }}
                dataSource={alerts}
                renderItem={this.renderItem}
              />
            </div>

            {/*<Button*/}
            {/*type="dashed"*/}
            {/*style={{ width: '100%', marginBottom: 8 }}*/}
            {/*icon="plus"*/}
            {/*onClick={this.showModal}*/}
            {/*ref={component => {*/}
            {/*this.addBtn = findDOMNode(component);*/}
            {/*}}*/}
            {/*>*/}
            {/*添加*/}
            {/*</Button>*/}
            {/*<MoreBtn current={item}/>,*/}
            {/*<List*/}
            {/*size="large"*/}
            {/*rowKey="id"*/}
            {/*loading={loading}*/}
            {/*pagination={paginationProps}*/}
            {/*dataSource={alerts}*/}
            {/*renderItem={item => (*/}
            {/*<List.Item*/}
            {/*actions={getActions(item)}*/}
            {/*>*/}
            {/*<List.Item.Meta*/}
            {/*avatar={<Avatar style={{ color: '#fff', backgroundColor: item.status == 0 ? 'red' : 'green' }}*/}
            {/*icon={item.status == 0 ? 'alert' : 'phone'} shape="square" size="large"/>}*/}
            {/*title={<a href={item.href}>{item.title}</a>}*/}
            {/*description={item.description}*/}
            {/*/>*/}
            {/*<ListContent style={{ justifyContent: 'flex-start' }} data={item}/>*/}
            {/*</List.Item>*/}
            {/*)}*/}
            {/*/>*/}
          </Card>
        </div>
        <Modal
          title={done ? null : `任务${current ? '编辑' : '添加'}`}
          className={styles.standardListForm}
          width={640}
          bodyStyle={done ? { padding: '72px 0' } : { padding: '28px 0 0' }}
          destroyOnClose
          visible={visible}
          {...modalFooter}
        >
          {getModalContent()}
        </Modal>
      </PageHeaderWrapper>
    );
  }
}

export default BasicList;
