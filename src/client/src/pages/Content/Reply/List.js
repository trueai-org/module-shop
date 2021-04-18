import React, { PureComponent, Fragment } from 'react';
import { connect } from 'dva';
import { Card, Input, Button, Modal, Form, notification, Popconfirm, Table } from 'antd';
import PageHeaderWrapper from '@/components/PageHeaderWrapper';
import Ellipsis from '@/components/Ellipsis';
import StandardTable from '@/components/StandardTable';
import moment from 'moment';

const FormItem = Form.Item;
const StatusFormat = ['', '待审核', '', '', '', '通过', '', '', '拒绝'];
@connect()
@Form.create()
class ReplyList extends PureComponent {
    state = {
        loading: false,
        visible: false,
        current: {},
        submitting: false,

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
            width: 160,
            render: (text, record) => (
                <Fragment>
                    <Button.Group>
                        <Button icon="eye" size="small" onClick={() => this.showEditModal(record)}></Button>
                        {
                            record.status == 8 || record.status == 5 ?
                                <Button style={{ color: "#1890ff" }} icon="rollback" size="small" onClick={() => this.updateStatus(record.id, 1)}></Button> :
                                <Fragment>
                                    <Button style={{ color: "#52c41a" }} icon="check" size="small" onClick={() => this.updateStatus(record.id, 5)}></Button>
                                    <Button type="danger" icon="close" size="small" onClick={() => this.updateStatus(record.id, 8)}></Button>
                                </Fragment>

                        }
                        <Popconfirm title="确定要删除吗？" onConfirm={() => this.deleteItem(record.id)}>
                            <Button icon="delete" type="danger" size="small"></Button>
                        </Popconfirm>
                    </Button.Group>
                </Fragment>
            )
        },

        {
            title: '用户',
            dataIndex: 'replierName',
            width: 120,
            sorter: true,
            render: (text) => <Ellipsis length={6} tooltip>{text}</Ellipsis>
        },
        {
            title: '用户回复',
            dataIndex: 'toUserName',
            width: 120,
            sorter: true,
            render: (text) => <Ellipsis length={6} tooltip>{text}</Ellipsis>
        },
        {
            title: '状态',
            dataIndex: 'status',
            width: 100,
            render: (val) => StatusFormat[val],
            sorter: true,
        },
        {
            title: '赞',
            dataIndex: 'supportCount',
            width: 60,
            sorter: true,
        },
        {
            title: '评论',
            dataIndex: 'comment',
            // width: 200,
            render: (text, record) => !text || text.length <= 20 ? <Ellipsis length={20}>{text}</Ellipsis> :
                <a onClick={() => { this.showEditModal(record) }}>[详情]</a>
        },
        {
            title: '创建时间',
            dataIndex: 'createdOn',
            sorter: true,
            width: 120,
            render: val => <span>{moment(val).format('YYYY-MM-DD')}</span>,
        },
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

    deleteItem = id => {
        this.setState({ loading: true, });
        const { dispatch } = this.props;
        const params = { id };
        new Promise(resolve => {
            dispatch({
                type: 'replies/del',
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

    updateStatus = (id, status) => {
        this.setState({ loading: true, });
        const { dispatch } = this.props;
        const params = { status, id };
        new Promise(resolve => {
            dispatch({
                type: 'replies/put',
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
                type: 'replies/grid',
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
        this.setState({
            pageNum: pagination.current,
            pageSize: pagination.pageSize,
            predicate: sorter.field ? sorter.field : this.state.predicate,
            reverse: sorter.order == 'descend',
        }, () => {
            this.handleSearch();
        });

        // var firstPage = this.state.predicate && sorter.field != this.state.predicate;
        // this.setState({
        //     pageNum: pagination.current,
        //     pageSize: pagination.pageSize
        // }, () => {
        //     if (sorter.field) {
        //         this.setState({
        //             predicate: sorter.field,
        //             reverse: sorter.order == 'descend'
        //         }, () => {
        //             if (firstPage)
        //                 this.handleSearchFirst();
        //             else
        //                 this.handleSearch();
        //         });
        //     } else {
        //         if (firstPage)
        //             this.handleSearchFirst();
        //         else
        //             this.handleSearch();
        //     }
        // });
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
        return (
            <PageHeaderWrapper title="回复列表">
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
                        // scroll={{ x: 800 }}
                        />
                    </Card>
                </div>
                <Modal
                    title={`回复详情`}
                    destroyOnClose
                    visible={this.state.visible}
                    footer={null}
                    onCancel={this.handleCancel}
                // {...modalFooter}
                >
                    {this.state.current.comment}
                </Modal>
            </PageHeaderWrapper>
        );
    }
}

export default ReplyList;
