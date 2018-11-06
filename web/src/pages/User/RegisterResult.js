import React, { Component } from 'react';
import { Button } from 'antd';
import Link from 'umi/link';
import Result from '@/components/Result';
import styles from './RegisterResult.less';
import router from 'umi/router';

const actions = (
  <div className={styles.actions}>
    <a href="">
      <Button size="large" type="primary">
        查看邮箱
      </Button>
    </a>
    <Link to="/">
      <Button size="large">返回首页</Button>
    </Link>
  </div>
);

export default class RegisterResult extends Component {
  constructor(props) {
    super(props);
    this.state = {
      description:"将在5秒后回到登录页"
    }
  }

  componentDidMount() {
    let x = 5;
    this.timer = setInterval(()=>{
      if(x > 0) {
        this.setState({
          description:`将在${x}秒后回到登录页`
        })
        x--;
      }
      else {
        clearInterval(this.timer);
        // router.push({
        //   pathname: '/user/login'
        // });
        location.href = "/";
      }
    },1000)
  }

  componentDidUnMount() {
    if(this.timer) {
      clearInterval(this.timer);
    }
  }

  render() {
    const {location} = this.props;
    const user = location.state.user;
    return (<Result
      className={styles.registerResult}
      type="success"
      title={
        <div className={styles.title}>
          {`你的账户：${user.username} 注册成功`}
        </div>
      }
      description={this.state.description}
      actions={actions}
      style={{ marginTop: 56 }}
    />)
  }
}

