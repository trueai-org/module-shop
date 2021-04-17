import React, { PureComponent, Fragment } from 'react';
import { connect } from 'dva';
import {
    Row, Col, List, Card, Input, Button, Modal, Form, notification, Table, Popconfirm, Divider, Select, Tag, Icon,
    Menu, Dropdown, Checkbox, Switch, Badge, Tooltip, InputNumber, TreeSelect
} from 'antd';

import PageHeaderWrapper from '@/components/PageHeaderWrapper';
import StandardTable from '@/components/StandardTable';
import router from 'umi/router';
import Link from 'umi/link';

const FormItem = Form.Item;
const level = ['省', '市', '区', '街道'];

@connect()
@Form.create()
class ProvinceList extends PureComponent {
    constructor(props) {
        super(props);
        this.state = {
            loading: false,
            visible: false,
            data: [],
            current: {},
            submitting: false,

            name: '',
            search: {},
            pageNum: 1,
            pageSize: 10,
            predicate: '',
            reverse: true,
            pageData: {
                list: [],
                pagination: {}
            },

            option: {},
            countryId: props.location.query.id,

            treeDataLoading: false,
            treeData: []
        };
    }

    columns = [
        {
            title: '操作',
            align: 'center',
            key: 'operation',
            width: 130,
            render: (text, record) => (
                <Fragment>
                    <Button.Group>
                        <Button size="small" onClick={() => this.showEditModal(record)}>编辑</Button>
                        <Popconfirm title="确定要删除吗？" onConfirm={() => this.deleteItem(record.id)}>
                            <Button type="danger" size="small">删除</Button>
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
            title: '类型',
            dataIndex: 'level',
            width: 110,
            sorter: true,
            filters: [
                {
                    text: level[0],
                    value: 0,
                },
                {
                    text: level[1],
                    value: 1,
                },
                {
                    text: level[2],
                    value: 2,
                },
                {
                    text: level[3],
                    value: 3,
                },
            ],
            render(val) {
                return level[val];
            },
        },
        {
            title: '编码',
            dataIndex: 'code',
            width: 110,
            sorter: true,
        },
        {
            title: '名称',
            dataIndex: 'name',
            sorter: true,
        },
        {
            title: '父级',
            dataIndex: 'parentName',
        },
        {
            title: '显示顺序',
            dataIndex: 'displayOrder',
            width: 110,
            sorter: true,
        },
        {
            title: '是否发布',
            dataIndex: 'isPublished',
            sorter: true,
            width: 110,
            render: (val) => this.boolFormat(val)
        }
    ];

    boolFormat(val) {
        //(val) => <Switch checked={val} disabled />,
        return <Icon style={{ color: val == true ? "#1890ff" : "#f5222d" }} type={val == true ? "check" : "close"} />;
    }

    componentDidMount() {
        this.handleInit();
        this.handleSearchFirst();
    }

    handleInit = () => {
        const { dispatch } = this.props;
        this.setState({ treeDataLoading: true });
        new Promise(resolve => {
            dispatch({
                type: 'province/tree',
                payload: {
                    resolve,
                    params: { countryId: this.state.countryId },
                },
            });
        }).then(res => {
            this.setState({ treeDataLoading: false });
            if (res.success === true) {
                this.setState({ treeData: res.data });
            } else {
                notification.error({ message: res.message, });
            }
        });
    }

    showModal = () => {
        this.setState({
            visible: true,
            current: {}
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
            visible: false
        });
    };

    handleSubmit = e => {
        e.preventDefault();
        const { dispatch, form } = this.props;
        const id = this.state.current ? this.state.current.id : '';

        form.validateFields((err, values) => {
            if (err) return;

            var params = {
                ...values,
                countryId: this.state.countryId
            };

            let bt = 'province/add';
            if (id) {
                params.id = id;
                bt = 'province/edit';
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
                    this.handleInit();
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
        this.setState({ loading: true, });
        const { dispatch } = this.props;
        new Promise(resolve => {
            dispatch({
                type: 'province/delete',
                payload: {
                    resolve,
                    params: { id },
                },
            });
        }).then(res => {
            this.setState({ loading: false });
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
            search: { ...this.state.search, name: this.state.name },
            countryId: this.state.countryId
        };

        new Promise(resolve => {
            dispatch({
                type: 'province/grid',
                payload: {
                    resolve,
                    params,
                },
            });
        }).then(res => {
            this.setState({ loading: false });
            if (res.success === true) {
                this.setState({ pageData: res.data });
            } else {
                notification.error({ message: res.message });
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

    renderSimpleForm() {
        return (
            <Form layout="inline" style={{ marginBottom: 20 }}>
                <Form.Item>
                    <Input
                        allowClear
                        placeholder="名称"
                        value={this.state.name}
                        onChange={(e) => {
                            this.setState({ name: e.target.value });
                        }}
                    />
                </Form.Item>
                <Form.Item>
                    <Button
                        onClick={this.handleSearch}
                        type="primary"
                        htmlType="submit"
                        icon="search"
                    >查询</Button>
                </Form.Item>
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
                    <FormItem label="名称" {...formLayout}>
                        {getFieldDecorator('name', {
                            rules: [{ required: true, message: '请输入名称' }],
                            initialValue: this.state.current.name || '',
                        })(<Input placeholder="请输入" />)}
                    </FormItem>
                    <FormItem label="编码" {...formLayout}>
                        {getFieldDecorator('code', {
                            initialValue: this.state.current.code || '',
                        })(<Input placeholder="请输入" />)}
                    </FormItem>
                    <FormItem label={<span>父级</span>} {...formLayout}>
                        {getFieldDecorator('parentId', {
                            initialValue: this.state.current.parentId || '', valuePropName: 'value'
                        })(
                            <TreeSelect
                                // treeDefaultExpandAll
                                loading={this.state.treeDataLoading}
                                allowClear={true}
                                treeData={this.state.treeData}
                            />)}
                    </FormItem>
                    <FormItem label="显示顺序" {...formLayout}>
                        {getFieldDecorator('displayOrder', {
                            initialValue: this.state.current.displayOrder || 0,
                        })(<InputNumber style={{ width: '100%' }} />)}
                    </FormItem>
                    <FormItem label="发布" {...formLayout}>
                        {getFieldDecorator('isPublished', {
                            initialValue: this.state.current.isPublished || false, valuePropName: 'checked'
                        })(<Checkbox />)}
                    </FormItem>
                </Form>
            );
        };
        const rollback = (
            <Fragment>
                <Button
                    onClick={this.showModal}
                    type="primary"
                    icon="plus">添加</Button>
                <Link to="./list">
                    <Button>
                        <Icon type="rollback" />
                    </Button>
                </Link>
            </Fragment>
        );
        return (
            <PageHeaderWrapper title="省市区" action={rollback}>
                <div>
                    <Card bordered={false}>
                        <div>
                            {this.renderSimpleForm()}
                        </div>
                        <StandardTable
                            pagination={pagination}
                            loading={this.state.loading}
                            data={this.state.pageData}
                            rowKey={record => record.id}
                            columns={this.columns}
                            bordered
                            onChange={this.handleStandardTableChange}
                            scroll={{ x: 960 }}
                        />
                    </Card>
                </div>
                <Modal
                    title={`省市区 - ${this.state.current.id ? '编辑' : '添加'}`}
                    destroyOnClose
                    visible={this.state.visible}
                    {...modalFooter}>
                    {getModalContent()}
                </Modal>
            </PageHeaderWrapper>
        );
    }
}

export default ProvinceList;
