import React, { PureComponent, Fragment } from 'react';
import { connect } from 'dva';
import { Card, Input, Button, Modal, Form, notification, Popconfirm, Table } from 'antd';
import PageHeaderWrapper from '@/components/PageHeaderWrapper';
import Ellipsis from '@/components/Ellipsis';

const FormItem = Form.Item;
const FormaterType = ['string', 'json'];
@connect()
@Form.create()
class SettingList extends PureComponent {
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
            width: 60,
            render: (text, record) => (
                <Fragment>
                    <Button.Group>
                        <Button icon="edit" size="small" onClick={() => this.showEditModal(record)}></Button>
                    </Button.Group>
                </Fragment>
            )
        },
        {
            title: 'Key',
            dataIndex: 'id',
            width: 120,
        },
        {
            title: '模块',
            dataIndex: 'module',
            width: 120,
        },

        {
            title: '值',
            dataIndex: 'value',
            width: 200,
            render: (text, record) => record.formatType == 0 ? <Ellipsis length={30}>{text}</Ellipsis> :
                <a onClick={() => { this.showEditModal(record) }}>[object]</a>
        },
        {
            title: '格式',
            dataIndex: 'formatType',
            width: 60,
            render: (val) => FormaterType[val]
        },
        {
            title: '类型',
            dataIndex: 'type',
            width: 160,
            render: (text) => <Ellipsis length={32} tooltip>{text}</Ellipsis>
        },
        {
            title: '备注',
            dataIndex: 'note',
        }
    ];

    componentDidMount() {
        this.handleSearch();
    }

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
        form.validateFields((err, values) => {
            if (err) return;

            let params = {
                ...values,
                id: this.state.current.id
            };

            if (this.state.submitting === true)
                return;
            this.setState({ submitting: true });
            new Promise(resolve => {
                dispatch({
                    type: 'appsetting/put',
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
                type: 'appsetting/delete',
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
                type: 'appsetting/get',
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
        return (
            <PageHeaderWrapper title="高级设置">
                <div>
                    <Card bordered={false}>
                        <Table
                            bordered
                            rowKey={record => record.id}
                            pagination={false}
                            dataSource={this.state.data}
                            columns={this.columns}
                            loading={this.state.loading}
                            scroll={{ x: 1100 }}
                        />
                    </Card>
                </div>
                <Modal
                    title={`高级设置 - 编辑值`}
                    destroyOnClose
                    visible={this.state.visible}
                    {...modalFooter}>
                    <Form onSubmit={this.handleSubmit}>
                        <FormItem
                        //  label="值"
                        >
                            {getFieldDecorator('value', {
                                rules: [{ required: true, message: '请输入值' }],
                                initialValue: this.state.current.value || '',
                            })(
                                <Input.TextArea rows={10} />
                            )}
                        </FormItem>
                    </Form>
                </Modal>
            </PageHeaderWrapper>
        );
    }
}

export default SettingList;
