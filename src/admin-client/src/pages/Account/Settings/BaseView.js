import React, { Component, Fragment } from 'react';
import { formatMessage, FormattedMessage } from 'umi/locale';
import { Form, Input, Upload, Select, Button, message } from 'antd';
import { connect } from 'dva';
import styles from './BaseView.less';
import PhoneView from './PhoneView';
// import { getTimeDistance } from '@/utils/utils';

const FormItem = Form.Item;
const { Option } = Select;

const validatorGeographic = (rule, value, callback) => {
  const { province, city } = value;
  if (!province.key) {
    callback('Please input your province!');
  }
  if (!city.key) {
    callback('Please input your city!');
  }
  callback();
};

const validatorPhone = (rule, value, callback) => {
  const values = value.split('-');
  if (!values[0]) {
    callback('Please input your area code!');
  }
  if (!values[1]) {
    callback('Please input your phone number!');
  }
  callback();
};

@connect(({ user }) => ({
  currentUser: user.currentUser,
}))
@Form.create()
class BaseView extends Component {
  state = {
    submitting: false,
    mediaId: undefined,
    mediaUrl: '',
    uploading: false
  };

  componentDidMount() {
    this.setBaseInfo();
  }

  setBaseInfo = () => {
    const { currentUser, form } = this.props;
    Object.keys(form.getFieldsValue()).forEach(key => {
      const obj = {};
      obj[key] = currentUser[key] || null;
      form.setFieldsValue(obj);
    });
  };

  getAvatarURL() {
    if (this.state.mediaUrl)
      return this.state.mediaUrl;
    const { currentUser } = this.props;
    if (currentUser.avatar) {
      return currentUser.avatar;
    }
    const url = 'https://gw.alipayobjects.com/zos/rmsportal/BiazfanxmamNRoxxVxka.png';
    return url;
  }

  handleSubmit = e => {
    e.preventDefault();
    const { form, dispatch } = this.props;
    form.validateFields({ force: true }, (err, values) => {
      if (err) {
        return;
      }
      this.setState({ submitting: true });
      new Promise(resolve => {
        dispatch({
          type: 'user/updateCurrent',
          payload: {
            resolve,
            params: {
              ...values,
              mediaId: this.state.mediaId,
            }
          },
        });
      }).then(res => {
        this.setState({ submitting: false });
        if (res.success === true) {
          message.info("更新基本信息成功");
        } else {
          message.warning(res.message);
        }
      });
    });
  };

  getViewDom = ref => {
    this.view = ref;
  };

  handleUpload = file => {
    const { dispatch } = this.props;
    const formData = new FormData();
    formData.append('file', file);
    this.setState({ uploading: true });
    new Promise(resolve => {
      dispatch({
        type: 'upload/uploadImage',
        payload: {
          resolve,
          params: formData,
        },
      });
    }).then(res => {
      this.setState({ uploading: false });
      if (res.success === true) {
        this.setState({
          mediaId: res.data.id,
          mediaUrl: res.data.url,
        });
      } else {
        message.error(res.message);
      }
    });
  };

  render() {
    const { form: { getFieldDecorator }, } = this.props;

    // 头像组件 方便以后独立，增加裁剪之类的功能
    const AvatarView = ({ avatar }) => (
      <Fragment>
        <div className={styles.avatar_title}>
          <FormattedMessage id="app.settings.basic.avatar" defaultMessage="Avatar" />
        </div>
        <div className={styles.avatar}>
          <img src={avatar} alt="avatar" />
        </div>
        <Upload
          fileList={[]}
          showUploadList={false}
          action={this.handleUpload}
        >
          <div className={styles.button_view}>
            <Button icon="upload" loading={this.state.uploading} >
              <FormattedMessage id="app.settings.basic.change-avatar" defaultMessage="Change avatar" />
            </Button>
          </div>
        </Upload>
      </Fragment>
    );

    return (
      <div className={styles.baseView} ref={this.getViewDom}>
        <div className={styles.left}>
          <Form layout="vertical" onSubmit={this.handleSubmit} hideRequiredMark>
            <FormItem label='昵称'>
              {getFieldDecorator('fullName', {
                rules: [
                  {
                    required: true,
                    message: '请输入您的昵称/全名',
                  },
                ],
              })(<Input />)}
            </FormItem>
            {/* <FormItem label='管理员备注'>
              {getFieldDecorator('adminRemark')(
                <Input.TextArea
                  placeholder='管理员备注'
                  rows={4}
                />
              )}
            </FormItem> */}
            <FormItem label='备注'>
              {getFieldDecorator('adminRemark')(
                <Input.TextArea
                  placeholder='备注'
                  rows={4}
                />
              )}
            </FormItem>
            <Button
              type="primary"
              htmlType="submit"
              loading={this.state.submitting}
            >
              <FormattedMessage
                id="app.settings.basic.update"
                defaultMessage="Update Information"
              />
            </Button>
          </Form>
        </div>
        <div className={styles.right}>
          <AvatarView avatar={this.getAvatarURL()} />
        </div>
      </div>
    );
  }
}

export default BaseView;
