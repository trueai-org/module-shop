import React, { PureComponent, Fragment } from 'react';
import { connect } from 'dva';
import {
    List, Card, Input, Button, Modal, Form, notification, Table, Popconfirm, Divider, Select, Tag, Icon,
    Redio, Menu, Dropdown, Checkbox, Switch
} from 'antd';

import PageHeaderWrapper from '@/components/PageHeaderWrapper';
import StandardTable from '@/components/StandardTable';
import router from 'umi/router';
import Link from 'umi/link';

const FormItem = Form.Item;
const Option = Select.Option;

@connect()
@Form.create()
class ProductAttributeListData extends PureComponent {
    constructor(props) {
        super(props);
        this.state = {
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

            attributeId: props.location.query.id,
        };
    }

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
                            {/* <a href="javascript:;">删除</a> */}
                        </Popconfirm>
                        {/* <Button size="small" onClick={() => this.deleteItem(record.id)}>删除</Button> */}
                    </Button.Group>
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
            title: '值',
            dataIndex: 'value',
            sorter: true,
            width: 200,
        },
        {
            title: '描述',
            dataIndex: 'description'
        },
        {
            title: '是否发布',
            dataIndex: 'isPublished',
            sorter: true,
            width: 120,
            render: (val) => <Switch checked={val} disabled />
        }
    ];

    componentDidMount() {
        this.handleInit();
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

    handleSubmit = e => {
        e.preventDefault();
        const { dispatch, form } = this.props;
        const id = this.state.current ? this.state.current.id : '';

        form.validateFields((err, values) => {
            if (err) return;

            var params = {
                ...values,
                attributeId: this.state.attributeId
            };

            let bt = 'attribute/addProductAttrData';
            if (id) {
                params.id = id;
                bt = 'attribute/editProductAttrData';
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
                type: 'attribute/deleteProductAttrData',
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
                // this.handleInit();
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

    handleInit = () => {
        const { dispatch } = this.props;
        // this.setState({ loading: true });
        // new Promise(resolve => {
        //     dispatch({
        //         type: 'attr/queryProductAttr',
        //         payload: { resolve }
        //     });
        // }).then(res => {
        //     this.setState({ loading: false });
        //     if (res.success === true) {
        //         if (res.data != null) {
        //             this.setState({
        //                 data: res.data
        //             });
        //         }
        //     } else {
        //         notification.error({
        //             message: res.message,
        //         });
        //     }
        // });

        this.setState({
            selectLoading: true
        });

        new Promise(resolve => {
            dispatch({
                type: 'group/queryProductAGS',
                payload: {
                    resolve,
                },
            });
        }).then(res => {
            if (res.success === true) {
                this.setState({
                    selectLoading: false,
                });
                let cs = [];
                let list = [];
                list = res.data;
                list.forEach(c => {
                    cs.push(<Option value={c.id} key={c.id}>{c.name}</Option>);
                });
                this.setState({ children: cs });
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
            attributeId: this.state.attributeId
        };

        new Promise(resolve => {
            dispatch({
                type: 'attribute/queryProductAttrDataGrid',
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
                    <FormItem label="属性值" {...formLayout}>
                        {getFieldDecorator('value', {
                            rules: [{ required: true, message: '请输入属性值' }],
                            initialValue: this.state.current.value || '',
                        })(<Input placeholder="请输入" />)}
                    </FormItem>
                    <FormItem label="描述" {...formLayout}>
                        {getFieldDecorator('description', {
                            initialValue: this.state.current.description || '',
                        })(<Input placeholder="请输入" />)}
                    </FormItem>
                    <FormItem label="发布" {...formLayout}>
                        {getFieldDecorator('isPublished', {
                            initialValue: this.state.current.isPublished || false, valuePropName: 'checked'
                        })(<Checkbox />)}
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
            <PageHeaderWrapper title="属性值" action={rollback}>
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
                    title={`属性值 - ${this.state.current.id ? '编辑' : '添加'}`}
                    destroyOnClose
                    visible={this.state.visible}
                    {...modalFooter}>
                    {getModalContent()}
                </Modal>
            </PageHeaderWrapper>
        );
    }
}

export default ProductAttributeListData;
