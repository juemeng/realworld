import React, { PureComponent,Fragment } from 'react';
import { connect } from 'dva';
import {
  Form,
  Input,
  DatePicker,
  Select,
  Button,
  Card,
  InputNumber,
  Radio,
  Icon,
  Tooltip,
  Table,
  Badge,
  Divider,
  Modal
} from 'antd';
import PageHeaderWrapper from '@/components/PageHeaderWrapper';
import styles from './style.less';
import tableStyles from './TableList.less';
import moment from 'moment/moment';

const FormItem = Form.Item;
const { Option } = Select;
const { RangePicker } = DatePicker;
const { TextArea } = Input;
const statusMap = ['default', 'success'];
const status = ['未绑定', '已上线', ];



const CreateForm = Form.create()(props => {
  const { modalVisible, form, handleDeploy, handleModalVisible,record,submitting } = props;
  const formItemLayout = {
    labelCol: {
      xs: { span: 24 },
      sm: { span: 7 },
    },
    wrapperCol: {
      xs: { span: 24 },
      sm: { span: 12 },
      md: { span: 10 },
    },
  };

  const submitFormLayout = {
    wrapperCol: {
      xs: { span: 24, offset: 0 },
      sm: { span: 10, offset: 7 },
    },
  };

  const { getFieldDecorator, getFieldValue } = form;

  const okHandle = () => {
    form.validateFields((err, fieldsValue) => {
      if (err) return;
      handleDeploy({...fieldsValue,floorId:record.floorId});
    });
  };
  return (
    <Modal
      destroyOnClose
      title={record.status == 0?`新建绑定：${record.floorInfo}`:`重新绑定：${record.floorInfo}`}
      visible={modalVisible}
      width={800}
      confirmLoading={submitting}
      onOk={okHandle}
      onCancel={() => handleModalVisible()}
    >
      {/*<Form onSubmit={this.handleSubmit} hideRequiredMark style={{ marginTop: 8 }}>*/}
        <FormItem
          {...formItemLayout}
          label={
            <span>
                  服务器地址
                  <em className={styles.optional}>
                    （必填）
                    <Tooltip title="待绑定服务器的地址,例如:192.168.1.200">
                      <Icon type="info-circle-o" style={{ marginRight: 4 }} />
                    </Tooltip>
                  </em>
                </span>
          }
        >
          {getFieldDecorator('host', {
            initialValue:record.host?record.host:'',
            rules: [
              {
                required: true,
                message: '请输入服务器地址',
              },
            ],
          })(
            <Input placeholder="请输入服务器地址，例如:192.168.1.200" />
          )}
        </FormItem>
        <FormItem
          {...formItemLayout}
          label={
            <span>
                  端口
                  <em className={styles.optional}>
                    （必填）
                    <Tooltip title="默认端口为：22">
                      <Icon type="info-circle-o" style={{ marginRight: 4 }} />
                    </Tooltip>
                  </em>
                </span>
          }
        >
          {getFieldDecorator('port', {
            initialValue:record.port?record.port:22,
            rules: [
              {
                required: true,
                message: '请输入端口号',
              },
            ],
          })(
            <InputNumber min={1} max={100000} />
          )}
        </FormItem>
        <FormItem
          {...formItemLayout}
          label={
            <span>
                  用户名
                  <em className={styles.optional}>
                    （必填）
                    <Tooltip title="需要提供服务器具有操作权限的用户名，如：root，树莓派默认用户为：pi">
                      <Icon type="info-circle-o" style={{ marginRight: 4 }} />
                    </Tooltip>
                  </em>
                </span>
          }
        >
          {getFieldDecorator('username', {
            initialValue:record.username?record.username:'pi',
            rules: [
              {
                required: true,
                message: '请输入用户名',
              },
            ],
          })(
            <Input placeholder="请输入用户名" />
          )}
        </FormItem>
        <FormItem
          {...formItemLayout}
          label={
            <span>
                  密码
                  <em className={styles.optional}>
                    （必填）
                    <Tooltip title="服务器用户的密码">
                      <Icon type="info-circle-o" style={{ marginRight: 4 }} />
                    </Tooltip>
                  </em>
                </span>
          }
        >
          {getFieldDecorator('password', {
            initialValue:record.password?record.password:'',
            rules: [
              {
                required: true,
                message: '请输入密码',
              },
            ],
          })(
            <Input type={'password'} placeholder="请输入密码" />
          )}
        </FormItem>
      {/*</Form>*/}
    </Modal>
  );
});

@connect(({ moniter,loading }) => ({
  bindings: moniter.bindings,
  finished:moniter.finished,
  submitting: loading.effects['moniter/bind'],
  fetching: loading.effects['moniter/fetch']
}))
@Form.create()
class Deploy extends PureComponent {
  state = {
    modalVisible:false,
    record:{}
  };

  columns = [
    {
      title: '楼层名称',
      dataIndex: 'floorInfo',
      sorter: true,
    },
    {
      title: '服务器ID',
      dataIndex: 'masterId',
      sorter: true,
    },
    {
      title: '绑定状态',
      dataIndex: 'status',
      filters: [
        {
          text: status[0],
          value: 0,
        },
        {
          text: status[1],
          value: 1,
        },
      ],
      render(val) {
        return <Badge status={statusMap[val]} text={status[val]} />;
      },
    },
    {
      title: '操作',
      render: (text, record) => (
        <Fragment>
          <a onClick={() => {this.deploy(record)}}>{record.status == 0?'绑定':'重新绑定'}</a>
          <Divider type="vertical" />
          {record.status == 1 && <a href="">更新配置</a>}
        </Fragment>
      ),
    },
  ];

  componentDidMount() {
    const { dispatch, form } = this.props;
    dispatch({
      type: 'moniter/fetch'
    });
  }

  deploy = record => {
    this.setState({
      modalVisible:true,
      record:record
    })
  }


  handleModalVisible = flag => {
    this.setState({
      modalVisible: !!flag,
    });
  };

  handleDeploy = data => {
    const { dispatch } = this.props;
    dispatch({
      type: 'moniter/bind',
      payload: {
        deployInfo:data
      }
    });
  }

  render() {
    const { submitting,fetching,bindings,finished } = this.props;
    const { modalVisible,record } = this.state;
    let modalOpen = modalVisible;
    if(finished) {
      modalOpen = false;
    }
    const parentMethods = {
      handleDeploy: this.handleDeploy,
      handleModalVisible: this.handleModalVisible,
    };

    return (
      <PageHeaderWrapper
        title="设备上线"
        content="在此页面将部署好的树莓派服务器与楼层绑定，以及发布服务程序"
      >
        <Card bordered={false}>
          <div className={tableStyles.tableList}>
            <Table
              loading={fetching}
              rowKey={'floorId'}
              dataSource={bindings}
              columns={this.columns}
              pagination={false}
            />
          </div>
        </Card>
        <CreateForm {...parentMethods} modalVisible={modalOpen} record={record} submitting={submitting}/>
      </PageHeaderWrapper>
    );
  }
}

export default Deploy;
