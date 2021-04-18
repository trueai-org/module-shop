import React, { PureComponent, Fragment } from 'react';
import { connect } from 'dva';
import { Card, Input, Button, Modal, Form, notification, Popconfirm, Table } from 'antd';
import PageHeaderWrapper from '@/components/PageHeaderWrapper';

const FormItem = Form.Item;

@connect()
@Form.create()
class UnitList extends PureComponent {
    state = {
        loading: false,
        visible: false,
        data: [],
        current: {},
        submitting: false
    };
    columns = [
        {
            title: '操作',
            align: 'center',
            key: 'operation',
            width: 120,
            render: (text, record) => (
                <Fragment>
                    <Button.Group>
                        <Button icon="edit" size="small" onClick={() => this.showEditModal(record)}></Button>
                        <Popconfirm title="确定要删除吗？" onConfirm={() => this.deleteItem(record.id)}>
                            <Button icon="delete" type="danger" size="small"></Button>
                        </Popconfirm>
                    </Button.Group>
                </Fragment>
            )
        },
        // {
        //     title: 'ID',
        //     dataIndex: 'id',
        //     width: 120
        // },
        {
            title: '单位名称',
            dataIndex: 'name',
        }
    ];

    componentDidMount() {
        this.handleSearch();
    }

    showModal = () => {
        this.setState({
            visible: true,
            current: {},
        });
    };

    showEditModal = item => {
        this.setState({
            visible: true,
            current: item,
        });
    };

    handleCancel = () => {
        this.setState({
            visible: false,
        });
    };

    handleSubmit = e => {
        e.preventDefault();
        const { dispatch, form } = this.props;
        const id = this.state.current ? this.state.current.id : '';

        form.validateFields((err, values) => {
            if (err) return;

            var params = {
                ...values
            };

            let bt = 'unit/add';
            if (id) {
                params.id = id;
                bt = 'unit/edit';
            }

            if (this.state.submitting === true)
                return;
            this.setState({ submitting: true });
            new Promise(resolve => {
                dispatch({
                    type: bt,
                    payload: {
                        resolve,
                        params
                    },
                });
            }).then(res => {
                this.setState({ submitting: false });
                if (res.success === true) {
                    form.resetFields();
                    this.setState({ visible: false });
                    this.handleSearch();
                } else {
                    notification.error({ message: res.message, });
                }
            });
        });
    };

    deleteItem = id => {
        this.setState({ loading: true, });
        const { dispatch } = this.props;
        const params = { id };
        new Promise(resolve => {
            dispatch({
                type: 'unit/delete',
                payload: {
                    resolve,
                    params,
                },
            });
        }).then(res => {
            this.setState({ loading: false, });
            if (res.success === true) {
                this.handleSearch();
            } else {
                notification.error({ message: res.message, });
            }
        });
    };

    handleSearch = () => {
        this.setState({ loading: true, });
        const { dispatch } = this.props;
        new Promise(resolve => {
            dispatch({
                type: 'unit/list',
                payload: {
                    resolve,
                    params: {},
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
        const { form: { getFieldDecorator }, } = this.props;
        const modalFooter = { okText: '保存', onOk: this.handleSubmit, onCancel: this.handleCancel };
        const formLayout = {
            labelCol: { span: 7 },
            wrapperCol: { span: 13 },
        };
        const action = (
            <Fragment>
                <Button
                    onClick={this.showModal}
                    type="primary"
                    icon="plus">添加</Button>
            </Fragment>
        );
        return (
            <PageHeaderWrapper title="商品单位" action={action}>
                <div>
                    <Card bordered={false}>
                        <Table
                            bordered
                            rowKey={record => record.id}
                            pagination={false}
                            dataSource={this.state.data}
                            columns={this.columns}
                            loading={this.state.loading}
                        />
                    </Card>
                </div>
                <Modal
                    title={`单位 - ${this.state.current.id ? '编辑' : '添加'}`}
                    destroyOnClose
                    visible={this.state.visible}
                    {...modalFooter}>
                    <Form onSubmit={this.handleSubmit}>
                        <FormItem label="名称" {...formLayout}>
                            {getFieldDecorator('name', {
                                rules: [{ required: true, message: '请输入单位名称' }],
                                initialValue: this.state.current.name || '',
                            })(<Input placeholder="请输入" />)}
                        </FormItem>
                    </Form>
                </Modal>
            </PageHeaderWrapper>
        );
    }
}

export default UnitList;
