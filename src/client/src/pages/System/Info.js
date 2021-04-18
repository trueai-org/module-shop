import React, { PureComponent, Fragment } from 'react';
import { connect } from 'dva';
import { Card, Input, Button, Modal, Form, notification, Popconfirm, Table } from 'antd';
import PageHeaderWrapper from '@/components/PageHeaderWrapper';

const FormItem = Form.Item;

@connect()
@Form.create()
class SystemInfo extends PureComponent {
    state = {
        loading: false,
        data: {},
    };

    componentDidMount() {
        this.handleSearch();
    }

    handleSearch = () => {
        this.setState({ loading: true, });
        const { dispatch } = this.props;
        new Promise(resolve => {
            dispatch({
                type: 'system/info',
                payload: {
                    resolve,
                },
            });
        }).then(res => {
            this.setState({ loading: false });
            if (res.success === true) {
                this.setState({ data: res.data });
            } else {
                notification.error({ message: res.message, });
            }
        });
    };

    render() {
        const formLayout = {
            labelCol: { span: 8 },
            wrapperCol: { span: 16 },
        };
        return (
            <PageHeaderWrapper title="系统信息">
                <div>
                    <Card bordered={false}>
                        <Form onSubmit={this.handleSubmit}>
                            <FormItem label="Shop Version" {...formLayout}>
                                {this.state.data.version}
                            </FormItem>
                            <FormItem label="Operating system" {...formLayout}>
                                {this.state.data.operatingSystem}
                            </FormItem>
                            <FormItem label="ASP.NET info" {...formLayout}>
                                {this.state.data.aspNetInfo}
                            </FormItem>
                            <FormItem label="Is full trust level" {...formLayout}>
                                {this.state.data.isFullTrust}
                            </FormItem>
                            <FormItem label="Server time zone" {...formLayout}>
                                {this.state.data.serverTimeZone}
                            </FormItem>
                            <FormItem label="Server local time" {...formLayout}>
                                {this.state.data.serverLocalTime}
                            </FormItem>
                            <FormItem label="Coordinated Universal Time (UTC)	" {...formLayout}>
                                {this.state.data.utcTime}
                            </FormItem>
                            <FormItem label="HTTP HOST" {...formLayout}>
                                {this.state.data.httpHost}
                            </FormItem>
                        </Form>
                    </Card>
                </div>
            </PageHeaderWrapper>
        );
    }
}

export default SystemInfo;
