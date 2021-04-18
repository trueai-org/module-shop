import React, { PureComponent, Fragment } from 'react';
import { connect } from 'dva';
import {
    List, Card, Input, Button, Modal, Form, notification, Table, Popconfirm, Divider, Select, Tag, Icon,
    Menu, Dropdown, Radio
} from 'antd';

import PageHeaderWrapper from '@/components/PageHeaderWrapper';
import StandardTable from '@/components/StandardTable';

import router from 'umi/router';
import Link from 'umi/link';

const FormItem = Form.Item;
const Option = Select.Option;

@connect()
@Form.create()
class ProductOptionList extends PureComponent {
    state = {
        loading: false,
        visible: false,
        data: [],
        current: {},
        submitting: false,

        children: [],

        pageNum: 1,
        pageSize: 5,
        predicate: '',
        reverse: true,
        pageData: {
            list: [],
            pagination: {}
        },
    };
    columns = [
        {
            title: '操作',
            align: 'center',
            key: 'operation',
            width: 120,
            render: (text, record) => (
                <Fragment>
                    <Dropdown.Button size="small" overlay={<Menu>
                        <Menu.Item onClick={() => this.showEditModal(record)}>编辑</Menu.Item>
                        <Menu.Item onClick={() => this.showDeleteModal(record)}>删除</Menu.Item>
                        {/* <Menu.Item>
                            <Popconfirm title="确定要删除吗？" onConfirm={() => this.deleteItem(record.id)}>
                                删除
                            </Popconfirm>
                        </Menu.Item> */}
                    </Menu>}
                    // placement="topRight"
                    >
                    
                        <a onClick={() => this.handleData(text, record)}>选项值</a>
                    </Dropdown.Button>
                    {/* <Button type="primary" size="small" onClick={() => this.showEditModal(record)}>编辑</Button> */}
                    {/* <Tag color="blue"><a onClick={() => this.showEditModal(record)}>编辑</a></Tag> */}
                    {/* <Divider type="vertical" /> */}
                    {/* <Popconfirm title="确定要删除吗？" onConfirm={() => this.deleteItem(record.id)}>
                        <Button type="danger" size="small">删除</Button>
                        <Tag color="red"><a href="javascript:;">删除</a></Tag>
                    </Popconfirm> */}
                    {/* <Divider type="vertical" /> */}
                    {/* <Button size="small" onClick={() => this.showEditModal(record)}>数据</Button> */}
                    {/* <Tag color="green"><a onClick={() => this.showEditModal(record)}>数据</a></Tag> */}
                </Fragment>
            )
        },
        // {
        //     title: 'ID',
        //     dataIndex: 'id',
        //     width: 120,
        //     sorter: true,
        //     defaultSortOrder: 'descend',
        // },
        {
            title: '类型',
            dataIndex: 'displayType',
            sorter: true,
            width: 120,
            // align: 'center',
            render: (val) => <Tag color={val == 1 ? 'blue' : ''}>{val == 1 ? '颜色' : '文本'}</Tag>
        },
        {
            title: '名称',
            dataIndex: 'name',
            sorter: true,
        }
    ];

    componentDidMount() {
        this.handleSearchFirst();
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

    handleData = (text, record) => {
        router.push({
            pathname: './data',
            query: {
                id: record.id,
            },
        });
    }

    handleSubmit = e => {
        e.preventDefault();
        const { dispatch, form } = this.props;
        const id = this.state.current ? this.state.current.id : '';

        form.validateFields((err, values) => {
            if (err) return;

            var params = {
                ...values
            };

            let bt = 'option/addProductOption';
            if (id) {
                params.id = id;
                bt = 'option/editProductOption';
            }

            // console.log(params);

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
                    notification.error({
                        message: res.message,
                    });
                }
            });
        });
    };

    deleteItem = id => {
        this.setState({
            loading: true,
        });
        const { dispatch } = this.props;
        const params = { id };
        new Promise(resolve => {
            dispatch({
                type: 'option/deleteProductOption',
                payload: {
                    resolve,
                    params,
                },
            });
        }).then(res => {
            this.setState({
                loading: false,
            });
            if (res.success === true) {
                this.handleSearch();
            } else {
                notification.error({
                    message: res.message,
                });
            }
        });
    };

    showDeleteModal = (item) => {
        Modal.confirm({
            title: '删除选项',
            content: '确定删除该选项吗？',
            okText: '确认',
            cancelText: '取消',
            onOk: () => this.deleteItem(item.id),
        });
    };

    handleSearch = () => {
        this.setState({
            loading: true,
        });
        const { dispatch } = this.props;
        const params =
        {
            pagination: {
                current: this.state.pageNum,
                pageSize: this.state.pageSize
            },
            sort: {
                predicate: this.state.predicate,
                reverse: this.state.reverse
            }
        };

        new Promise(resolve => {
            dispatch({
                type: 'option/queryProductOptionGrid',
                payload: {
                    resolve,
                    params,
                },
            });
        }).then(res => {
            if (res.success === true) {
                this.setState({
                    loading: false,
                    pageData: res.data
                });
            } else {
                notification.error({
                    message: res.message,
                });
            }
        });
    };

    handleSearchFirst = () => {
        this.setState({
            pageNum: 1
        }, () => {
            this.handleSearch();
        });
    }

    handleStandardTableChange = (pagination, filtersArg, sorter) => {
        var firstPage = this.state.predicate && sorter.field != this.state.predicate;
        this.setState({
            pageNum: pagination.current,
            pageSize: pagination.pageSize
        }, () => {
            if (sorter.field) {
                this.setState({
                    predicate: sorter.field,
                    reverse: sorter.order == 'descend'
                }, () => {
                    if (firstPage)
                        this.handleSearchFirst();
                    else
                        this.handleSearch();
                });
            } else {
                if (firstPage)
                    this.handleSearchFirst();
                else
                    this.handleSearch();
            }
        });
    };

    render() {
        const { form: { getFieldDecorator }, } = this.props;
        const modalFooter = { okText: '保存', onOk: this.handleSubmit, onCancel: this.handleCancel };
        const extraContent = (
            <div>
                <Button
                    onClick={this.showModal}
                    type="primary"
                    icon="plus">
                    新增</Button>
            </div>
        );
        const formLayout = {
            labelCol: { span: 7 },
            wrapperCol: { span: 13 },
        };
        const pagination = {
            showQuickJumper: true,
            showSizeChanger: true,
            pageSizeOptions: ['5', '10', '50', '100'],
            defaultPageSize: this.state.pageSize,
            defaultCurrent: this.state.pageNum,
            current: this.state.pageNum,
            pageSize: this.state.pageSize,
            total: this.state.pageData.pagination.total || 0,
            showTotal: (total, range) => {
                return `${range[0]}-${range[1]} 条 , 共 ${total} 条`;
            }
        };
        const getModalContent = () => {
            return (
                <Form onSubmit={this.handleSubmit}>
                    <FormItem label="类型" {...formLayout}>
                        {getFieldDecorator('displayType', {
                            rules: [{ required: true, message: '显示类型' }],
                            initialValue: this.state.current.displayType || 0,
                        })(<Radio.Group>
                            <Radio value={0}>文本</Radio>
                            <Radio value={1}>颜色</Radio>
                        </Radio.Group>)}
                    </FormItem>
                    <FormItem label="名称" {...formLayout}>
                        {getFieldDecorator('name', {
                            rules: [{ required: true, message: '请输入选项名称' }],
                            initialValue: this.state.current.name || '',
                        })(<Input placeholder="请输入" />)}

                    </FormItem>
                </Form>
            );
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
            <PageHeaderWrapper title="商品选项" action={action}>
                <div>
                    <Card bordered={false}
                    // extra={extraContent}
                    >
                        {/* <div style={{ marginBottom: '20px' }} >
                            {action}
                        </div> */}
                        <StandardTable
                            pagination={pagination}
                            loading={this.state.loading}
                            data={this.state.pageData}
                            rowKey={record => record.id}
                            columns={this.columns}
                            bordered
                            onChange={this.handleStandardTableChange}
                        // scroll={{ x: 800 }}
                        />
                        {/* <Table bordered
                            rowKey={record => record.id}
                            pagination={false}
                            loading={this.state.loading}
                            dataSource={this.state.data}
                            columns={this.columns}
                        /> */}
                    </Card>
                </div>
                <Modal
                    title={`商品选项 - ${this.state.current.id ? '编辑' : '添加'}`}
                    destroyOnClose
                    visible={this.state.visible}
                    {...modalFooter}>
                    {getModalContent()}
                </Modal>
            </PageHeaderWrapper>
        );
    }
}

export default ProductOptionList;