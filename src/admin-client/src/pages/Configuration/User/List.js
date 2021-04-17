import React, { PureComponent, Fragment } from 'react';
import { connect } from 'dva';
import {
    Row, Col, List, Card, Input, Button, Modal, Form, notification, Table, Popconfirm, Divider, Select, Tag, Icon,
    Redio, Menu, Dropdown, Tooltip, Checkbox
} from 'antd';
import moment from 'moment';
import PageHeaderWrapper from '@/components/PageHeaderWrapper';
import StandardTable from '@/components/StandardTable';

import router from 'umi/router';
import Link from 'umi/link';
import { formatBool } from '@/utils/utils';

const role = ['null', 'admin', 'customer', 'guest'];
const roleMap = ['', 'red', 'blue', ''];

const FormItem = Form.Item;
const Option = Select.Option;

@connect()
@Form.create()
class UserList extends PureComponent {
    state = {
        loading: false,
        visible: false,
        data: [],
        current: {},
        submitting: false,
        selectLoading: false,
        children: [],

        search: {},
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
        //     width: 100,
        //     sorter: true,
        //     defaultSortOrder: 'descend',
        // },
        {
            title: '用户名',
            dataIndex: 'userName',
            // width: 120,
            sorter: true,
        },
        {
            title: '全名',
            dataIndex: 'fullName',
            // width: 120,
            sorter: true,
        },
        {
            title: '角色',
            dataIndex: 'roleIds',
            // width: 120,
            filters: [
                {
                    text: role[1],
                    value: 1,
                },
                {
                    text: role[2],
                    value: 2,
                },
                {
                    text: role[3],
                    value: 3,
                },
            ],
            render: (val) => val.map(c => {
                return <Tag key={c} color={roleMap[c]} >{role[c]}</Tag>;
            })
        },
        {
            title: '邮箱',
            dataIndex: 'email',
            //  width: 120,
            sorter: true,
        },
        {
            title: '电话',
            dataIndex: 'phoneNumber',
            // width: 200,
            sorter: true,
        },
        {
            title: '最后活动时间',
            dataIndex: 'lastActivityOn',
            width: 150,
            sorter: true,
            render: val => <span>{val ? moment(val).format('YYYY-MM-DD') : null}</span>,
        },
        {
            title: '已启用',
            dataIndex: 'isActive',
            width: 100,
            sorter: true,
            render: (val) => formatBool(val)
        },

        {
            title: '创建时间',
            dataIndex: 'createdOn',
            width: 120,
            sorter: true,
            render: val => <span>{moment(val).format('YYYY-MM-DD')}</span>,
        },
        {
            title: '更新时间',
            dataIndex: 'updatedOn',
            width: 120,
            sorter: true,
            render: val => <span>{moment(val).format('YYYY-MM-DD')}</span>,
        },

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
            pathname: '/catalog/product-attribute/data',
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

            let bt = 'userManager/add';
            if (id) {
                params.id = id;
                bt = 'userManager/edit';
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
        this.setState({ loading: true });
        const { dispatch } = this.props;
        const params = { id };
        new Promise(resolve => {
            dispatch({
                type: 'userManager/delete',
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
                notification.error({
                    message: res.message,
                });
            }
        });
    };

    showDeleteModal = (item) => {
        Modal.confirm({
            title: '删除属性',
            content: '确定删除该属性吗？',
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
            },
            search: this.state.search
        };

        new Promise(resolve => {
            dispatch({
                type: 'userManager/grid',
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
            pageSize: pagination.pageSize,
            search: {
                ...filtersArg
            }
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

    handleFormReset = () => {
        const { form, dispatch } = this.props;
        form.resetFields();
        this.setState({
            formValues: {},
        });
    };

    renderForm() {
        return (
            <Form layout="inline">
                <Row gutter={{ md: 8, lg: 24, xl: 48 }} style={{ marginBottom: 12, marginTop: 12 }}>
                    <Col md={8} sm={24}>
                        <Input
                            onChange={(e) => {
                                this.setState({
                                    search: Object.assign({}, this.state.search, { name: e.target.value })
                                });
                            }}
                            allowClear
                            placeholder="用户名/全名" />
                    </Col>
                    <Col md={8} sm={24}>
                        <Input
                            onChange={(e) => {
                                this.setState({
                                    search: Object.assign({}, this.state.search, { contact: e.target.value })
                                });
                            }}
                            allowClear
                            placeholder="联系方式(邮箱/电话)" />
                    </Col>
                    <Col md={8} sm={24}>
                        <Select
                            onChange={(e) => {
                                this.setState({
                                    search: Object.assign({}, this.state.search, { isActive: e })
                                });
                            }}
                            style={{ width: 100, paddingRight: 8, }}
                            placeholder="已启用"
                            allowClear>
                            <Option value='false'>否</Option>
                            <Option value='true'>是</Option>
                        </Select>
                        <Button onClick={this.handleSearch} type="primary" icon="search">查询</Button>
                    </Col>
                </Row>

            </Form>
        );
    }


    render() {
        const { form: { getFieldDecorator }, } = this.props;
        const modalFooter = { okText: '保存', onOk: this.handleSubmit, onCancel: this.handleCancel };
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
                    <FormItem label="用户名" {...formLayout}>
                        {getFieldDecorator('userName', {
                            rules: [{ required: true, message: '请输入用户名' }],
                            initialValue: this.state.current.userName || '',
                        })(<Input placeholder="请输入" />)}
                    </FormItem>
                    <FormItem label="全名" {...formLayout}>
                        {getFieldDecorator('fullName', {
                            rules: [{ required: true, message: '请输入全名（昵称）' }],
                            initialValue: this.state.current.fullName || '',
                        })(<Input placeholder="请输入" />)}
                    </FormItem>
                    <FormItem label="邮箱" {...formLayout}>
                        {getFieldDecorator('email', {
                            rules: [{ required: true, message: '请输入邮箱' }],
                            initialValue: this.state.current.email || '',
                        })(<Input placeholder="请输入" />)}
                    </FormItem>
                    <FormItem
                        label={<span>密码<Tooltip title="修改密码：请输入新密码"><Icon type="question-circle" theme="filled" /></Tooltip></span>}
                        {...formLayout}>
                        {getFieldDecorator('password', {
                            initialValue: this.state.current.password || '',
                        })(<Input placeholder="请输入" />)}
                    </FormItem>
                    <FormItem label="角色" {...formLayout}>
                        {getFieldDecorator('roleIds', {
                            rules: [{ required: true, message: '请选择角色' }],
                            initialValue: this.state.current.roleIds, valuePropName: 'value'
                        })(
                            <Select
                                mode="multiple"
                                loading={this.state.selectLoading}
                                allowClear={true}>
                                <Option value={1}>admin</Option>
                                <Option value={2}>customer</Option>
                                <Option value={3}>guest</Option>
                            </Select>)}
                    </FormItem>
                    <FormItem
                        label={<span>已启用</span>}
                        {...formLayout}>
                        {getFieldDecorator('isActive', {
                            initialValue: this.state.current.isActive || false, valuePropName: 'checked'
                        })(
                            <Checkbox />
                        )}
                    </FormItem>
                    <FormItem
                        label={<span>管理员备注<Tooltip title="管理员备注，仅内部使用"><Icon type="question-circle" theme="filled" /></Tooltip></span>}
                        {...formLayout}>
                        {getFieldDecorator('adminRemark', { initialValue: this.state.current.adminRemark })(
                            <Input.TextArea
                                style={{ minHeight: 32 }}
                                placeholder="管理员备注"
                                rows={2} />
                        )}
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
            <PageHeaderWrapper title="用户 - 列表" action={action}>
                <div>
                    <Card bordered={false}>
                        <div>{this.renderForm()}</div>
                        <StandardTable
                            pagination={pagination}
                            loading={this.state.loading}
                            data={this.state.pageData}
                            rowKey={record => record.id}
                            columns={this.columns}
                            bordered
                            onChange={this.handleStandardTableChange}
                            scroll={{ x: 1500 }}
                        />
                    </Card>
                </div>
                <Modal
                    title={`用户 - ${this.state.current.id ? '编辑' : '添加'}`}
                    destroyOnClose
                    visible={this.state.visible}
                    {...modalFooter}>
                    {getModalContent()}
                </Modal>
            </PageHeaderWrapper>
        );
    }
}

export default UserList;
