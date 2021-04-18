import React, { PureComponent, Fragment } from 'react';
import { connect } from 'dva';
import {
    Row, Col, List, Card, Input, Button, Modal, Form, notification, Table, Popconfirm, Divider, Select, Tag, Icon,
    Redio, Menu, Dropdown, Tooltip, Checkbox, TreeSelect
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
        current: {},
        submitting: false,

        search: {},
        pageNum: 1,
        pageSize: 5,
        predicate: 'updatedOn',
        reverse: true,
        pageData: {
            list: [],
            pagination: {}
        },

        countries: [],
        countriesLoading: false,

        provinces: [],
        provincesLoading: false,
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
            title: '仓库名称',
            dataIndex: 'name',
            sorter: true,
        },
        // {
        //     title: '联系人',
        //     dataIndex: 'contactName',
        //     sorter: true,
        // },
        {
            title: '管理员备注',
            dataIndex: 'adminRemark',
            sorter: true,
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
            defaultSortOrder: 'descend',
            render: val => <span>{moment(val).format('YYYY-MM-DD')}</span>,
        }
    ];

    componentDidMount() {
        this.handleInit();
        this.handleSearchFirst();
    }

    handleInit = () => {
        const { dispatch } = this.props;
        this.setState({ countriesLoading: true });
        new Promise(resolve => {
            dispatch({
                type: 'system/countries',
                payload: {
                    resolve
                },
            });
        }).then(res => {
            this.setState({ countriesLoading: false });
            if (res.success === true) {
                this.setState({ countries: res.data });
            } else {
                notification.error({ message: res.message, });
            }
        });
    }

    handleInitProvinces = (countryId) => {
        const { dispatch } = this.props;
        this.setState({ provincesLoading: true });
        new Promise(resolve => {
            dispatch({
                type: 'system/provinces',
                payload: {
                    resolve,
                    params: { countryId: countryId },
                },
            });
        }).then(res => {
            this.setState({ provincesLoading: false });
            if (res.success === true) {
                this.setState({ provinces: res.data });
            } else {
                notification.error({ message: res.message, });
            }
        });
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
        }, () => {
            this.handleInitProvinces(item.countryId);
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

            let bt = 'warehouse/add';
            if (id) {
                params.id = id;
                bt = 'warehouse/edit';
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
                type: 'warehouse/delete',
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
                type: 'warehouse/grid',
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
                    <FormItem label="仓库名称" {...formLayout}>
                        {getFieldDecorator('name', {
                            rules: [{ required: true, message: '请输入仓库名称' }],
                            initialValue: this.state.current.name || '',
                        })(<Input placeholder="请输入" />)}
                    </FormItem>
                    <FormItem label="联系人" {...formLayout}>
                        {getFieldDecorator('contactName', {
                            initialValue: this.state.current.contactName || '',
                        })(<Input placeholder="请输入" />)}
                    </FormItem>
                    <FormItem label="电话" {...formLayout}>
                        {getFieldDecorator('phone', {
                            initialValue: this.state.current.phone || '',
                        })(<Input placeholder="请输入" />)}
                    </FormItem>
                    <FormItem label="国家" {...formLayout}>
                        {getFieldDecorator('countryId', {
                            rules: [{ required: true, message: '请选择国家' }],
                            initialValue: this.state.current.countryId, valuePropName: 'value'
                        })(
                            <Select
                                placeholder="请选择国家"
                                loading={this.state.countriesLoading}
                                allowClear={true}
                                onChange={(value) => {
                                    this.props.form.setFieldsValue({ stateOrProvinceId: '' })
                                    if (value) {
                                        this.handleInitProvinces(value);
                                    } else {
                                        this.setState({ provinces: [] });
                                    }
                                }}>
                                {this.state.countries.map(c => {
                                    return <Option key={c.id} value={c.id}>{c.name}</Option>;
                                })}
                            </Select>)}
                    </FormItem>
                    <FormItem label={<span>省市区</span>} {...formLayout}>
                        {getFieldDecorator('stateOrProvinceId', {
                            rules: [{ required: true, message: '请选择省市区' }],
                            initialValue: this.state.current.stateOrProvinceId || '', valuePropName: 'value'
                        })(
                            <TreeSelect
                                // value={this.state.currentProvinceId}
                                // treeDefaultExpandAll
                                loading={this.state.provincesLoading}
                                allowClear={true}
                                treeData={this.state.provinces}
                            />)}
                    </FormItem>
                    <FormItem label="城市" {...formLayout}>
                        {getFieldDecorator('city', {
                            initialValue: this.state.current.city || '',
                        })(<Input placeholder="请输入" />)}
                    </FormItem>
                    <FormItem label="邮政编码" {...formLayout}>
                        {getFieldDecorator('zipCode', {
                            initialValue: this.state.current.zipCode || '',
                        })(<Input placeholder="请输入" />)}
                    </FormItem>
                    <FormItem label="地址1" {...formLayout}>
                        {getFieldDecorator('addressLine1', {
                            initialValue: this.state.current.addressLine1 || '',
                        })(<Input placeholder="请输入" />)}
                    </FormItem>
                    <FormItem label="地址2" {...formLayout}>
                        {getFieldDecorator('addressLine2', {
                            initialValue: this.state.current.addressLine2 || '',
                        })(<Input placeholder="请输入" />)}
                    </FormItem>
                    <FormItem
                        label={<span>管理员备注<Tooltip title="管理员备注，仅内部使用"><Icon type="question-circle" theme="filled" /></Tooltip></span>}
                        {...formLayout}>
                        {getFieldDecorator('adminRemark', { initialValue: this.state.current.adminRemark })(
                            <Input.TextArea

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
            <PageHeaderWrapper title="仓库 - 列表" action={action}>
                <div>
                    <Card bordered={false}>
                        <StandardTable
                            pagination={pagination}
                            loading={this.state.loading}
                            data={this.state.pageData}
                            rowKey={record => record.id}
                            columns={this.columns}
                            bordered
                            onChange={this.handleStandardTableChange}
                        // scroll={{ x: 1500 }}
                        />
                    </Card>
                </div>
                <Modal
                    title={`仓库 - ${this.state.current.id ? '编辑' : '添加'}`}
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
