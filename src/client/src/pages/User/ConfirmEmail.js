import React, { Component } from 'react';
import { connect } from 'dva';
import { formatMessage, FormattedMessage } from 'umi/locale';
import Link from 'umi/link';
import router from 'umi/router';
import { Button, message } from 'antd';
import styles from './Register.less';

@connect()
class ConfirmEmail extends Component {
    constructor(props) {
        super(props);
        this.state = {
            id: props.location.query.id,
            code: props.location.query.code,
            email: props.location.query.email,
            submitting: false,
            success: false
        };
    }

    componentDidMount() {
        this.handleSubmit();
    }

    handleSubmit = () => {
        const { dispatch } = this.props;
        this.setState({ submitting: true });
        new Promise(resolve => {
            dispatch({
                type: 'user/confirmEmail',
                payload: {
                    resolve,
                    params: {
                        userId: this.state.id,
                        code: this.state.code
                    },
                },
            });
        }).then(res => {
            this.setState({ submitting: false });
            if (res.success === true) {
                this.setState({ success: true });
                message.info('邮箱激活成功,此邮箱可以作为登录账号!');
            } else {
                message.error(res.message);
            }
        });
    };

    render() {
        return (
            <div className={styles.main}>
                <h3 style={{ marginBottom: 60, marginTop: 60 }}>邮箱激活验证：{this.state.email}</h3>
                <Button
                    disabled={this.state.success}
                    size="large"
                    loading={this.state.submitting}
                    className={styles.submit}
                    type="primary"
                    htmlType="submit"
                    onClick={this.handleSubmit}
                >
                    {this.state.success ? '验证成功' : this.state.submitting ? '验证中' : '立即验证'}
                </Button>
                <Link className={styles.login} to="/user/login">返回登录</Link>
            </div>
        );
    }
}

export default ConfirmEmail;
