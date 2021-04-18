import React, { PureComponent, Fragment } from 'react';
import { connect } from 'dva';
import {
    List, Card, Input, Button, Modal, Form, notification, Table, Popconfirm, Divider, Select, Tag, Icon,
    Redio, Menu, Dropdown, Tooltip
} from 'antd';

import PageHeaderWrapper from '@/components/PageHeaderWrapper';
import StandardTable from '@/components/StandardTable';

import router from 'umi/router';
import Link from 'umi/link';

const FormItem = Form.Item;
const Option = Select.Option;

@connect()
@Form.create()
class CountryList extends PureComponent {
    state = {
        loading: false,
        visible: false,
        data: [],
        current: {},
        submitting: false,
        selectLoading: false,
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
            width: 130,
            render: (text, record) => (
                <Fragment>
                    <Button.Group>
                        <Tooltip title="省市区">
                            <Button icon="eye" size="small" onClick={() => this.handleData(text, record)}></Button>
                        </Tooltip>
                        <Button icon="edit" size="small" onClick={() => this.handleEdit(record.id)}></Button>
                        <Popconfirm title="确定要删除吗？" onConfirm={() => this.deleteItem(record.id)}>
                            <Button icon="delete" type="danger" size="small"></Button>
                        </Popconfirm>
                    </Button.Group>
                    {/* <Dropdown.Button size="small" overlay={
                        <Menu>
                            <Menu.Item onClick={() => this.handleEdit(record.id)}>编辑</Menu.Item>
                            <Menu.Item onClick={() => this.showDeleteModal(record)}>删除</Menu.Item>
                        </Menu>}>
                        <a onClick={() => this.handleData(text, record)}>省市区</a>
                    </Dropdown.Button> */}
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
            title: '名称',
            dataIndex: 'name',
            sorter: true,
        },
        {
            title: 'ISO',
            dataIndex: 'twoLetterIsoCode',
            width: 120,
            sorter: true,
            render: (text, record) => (
                <Fragment>
                    <span>{text}, </span>
                    <span>{record.threeLetterIsoCode}, </span>
                    <span>{record.numericIsoCode}</span>
                </Fragment>
            )
        },
        {
            title: '已发布',
            dataIndex: 'isPublished',
            width: 110,
            sorter: true,
            render: (val) => this.boolFormat(val)
        },
        {
            title: '省数量',
            dataIndex: 'stateOrProvinceCount',
            width: 110,
            // sorter: true,
        },
        {
            title: '显示顺序',
            dataIndex: 'displayOrder',
            width: 110,
            sorter: true,
        },
        {
            title: '允许账单',
            dataIndex: 'isBillingEnabled',
            width: 110,
            sorter: true,
            render: (val) => this.boolFormat(val)
        },
        {
            title: '允许配送',
            dataIndex: 'isShippingEnabled',
            width: 110,
            sorter: true,
            render: (val) => this.boolFormat(val)
        },
        {
            title: '启用市',
            dataIndex: 'isCityEnabled',
            width: 110,
            sorter: true,
            render: (val) => this.boolFormat(val)
        },
        {
            title: '启用区',
            dataIndex: 'isDistrictEnabled',
            width: 110,
            sorter: true,
            render: (val) => this.boolFormat(val)
        },
    ];

    boolFormat(val) {
        //(val) => <Switch checked={val} disabled />,
        return <Icon style={{ color: val == true ? "#1890ff" : "#f5222d" }} type={val == true ? "check" : "close"} />;
    }

    componentDidMount() {
        // this.handleInit();
        this.handleSearchFirst();
    }

    handleAdd = () => {
        router.push('./add');
    }

    handleEdit = (id) => {
        router.push({
            pathname: './edit',
            query: {
                id: id,
            },
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
        });
    };

    handleCancel = () => {
        this.setState({
            visible: false,
        });
    };

    handleData = (text, record) => {
        router.push({
            pathname: './province',
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

            let bt = 'attribute/addProductAttr';
            if (id) {
                params.id = id;
                bt = 'attribute/editProductAttr';
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
                    // this.handleInit();
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
                type: 'country/delete',
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
            title: '删除国家',
            content: '确定删除该国家吗？',
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
                type: 'country/grid',
                payload: {
                    resolve,
                    params,
                },
            });
        }).then(res => {
            this.setState({ loading: false });
            if (res.success === true) {
                this.setState({
                    pageData: res.data
                });
            } else {
                notification.error({ message: res.message, });
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
        const action = (
            <Fragment>
                <Button
                    onClick={this.handleAdd}
                    type="primary"
                    icon="plus">添加</Button>
            </Fragment>
        );
        return (
            <PageHeaderWrapper title="国家 - 列表" action={action}>
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
                            scroll={{ x: 1300 }}
                        />
                    </Card>
                </div>
            </PageHeaderWrapper>
        );
    }
}

export default CountryList;
