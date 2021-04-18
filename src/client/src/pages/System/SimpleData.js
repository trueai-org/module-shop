import React, { PureComponent, Fragment } from 'react';
import { connect } from 'dva';
import { Card, Input, Button, Modal, Form, notification, Popconfirm, Table } from 'antd';
import PageHeaderWrapper from '@/components/PageHeaderWrapper';

const FormItem = Form.Item;

@connect()
@Form.create()
class SimpleData extends PureComponent {
    state = {
        loading: false,
        data: {},
    };

    componentDidMount() {

    }

    handleSubmit = () => {
        this.setState({ loading: true, });
        const { dispatch } = this.props;
        new Promise(resolve => {
            dispatch({
                type: 'simpledata/reset',
                payload: {
                    resolve,
                },
            });
        }).then(res => {
            this.setState({ loading: false });
            if (res.success === true) {
                notification.info({ message: '重置样本数据成功' });
            } else {
                notification.error({ message: res.message });
            }
        });
    };

    render() {
        return (
            <PageHeaderWrapper title="样本数据管理">
                <div>
                    <Card bordered={false}>
                        <Popconfirm title="现有数据将被清空！确定要重置样本数据吗？" onConfirm={() => this.handleSubmit()}>
                            <Button type="danger" icon="reset" htmlType="submit" loading={this.state.loading}>重置样本数据</Button>
                        </Popconfirm>
                    </Card>
                </div>
            </PageHeaderWrapper>
        );
    }
}

export default SimpleData;
