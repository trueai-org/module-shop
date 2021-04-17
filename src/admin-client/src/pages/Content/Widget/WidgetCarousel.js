import React, { PureComponent, Fragment } from 'react';
import { connect } from 'dva';
import moment from 'moment';
import {
    Form, Input, Select, Button, Card, InputNumber, Icon,
    Checkbox, Upload, Modal, notification, Spin, DatePicker,
    Table, Radio, Avatar
} from 'antd';
import PageHeaderWrapper from '@/components/PageHeaderWrapper';
import router from 'umi/router';
import Link from 'umi/link';

const FormItem = Form.Item;
const { TextArea } = Input;
const Option = Select.Option;

const WidgetZoneWithId = [
    { key: 1, value: "Home Featured", desc: "首页头部" },
    { key: 2, value: "Home Main Content", desc: "首页主体" },
    { key: 3, value: "Home After Main Content", desc: "首页主体之后" }
];

@connect()
@Form.create()
class WidgetCarousel extends PureComponent {
    constructor(props) {
        super(props);
        this.state = {
            id: props.location.query.id,
            loading: false,
            data: {},
            submitting: false,

            // 图片
            images: [],
            imagesLoading: false,
        };
    }

    columnsImage = [
        {
            title: '标题',
            dataIndex: 'caption',
            render: (text, record) => (
                <Fragment>
                    <Input
                        onChange={e => {
                            record.caption = e.target.value;
                        }}
                        defaultValue={text} />
                </Fragment>
            ),
        },
        {
            title: '子标题',
            dataIndex: 'subCaption',
            render: (text, record) => (
                <Fragment>
                    <Input
                        onChange={e => {
                            record.subCaption = e.target.value;
                        }}
                        defaultValue={text} />
                </Fragment>
            ),
        },
        {
            title: '链接文本',
            dataIndex: 'linkText',
            render: (text, record) => (
                <Fragment>
                    <Input
                        onChange={e => {
                            record.linkText = e.target.value;
                        }}
                        defaultValue={text} />
                </Fragment>
            ),
        },
        {
            title: '链接地址',
            dataIndex: 'targetUrl',
            render: (text, record) => (
                <Fragment>
                    <Input
                        onChange={e => {
                            record.targetUrl = e.target.value;
                        }}
                        defaultValue={text} />
                </Fragment>
            ),
        },
        {
            title: '图片',
            dataIndex: 'imageUrl',
            align: 'center',
            width: 64,
            fixed: 'right',
            render: (text, record) => (
                <Fragment>
                    <Upload
                        action={(file) => {
                            this.handleUploadMain(file, record);
                        }}
                        listType="picture-card"
                        showUploadList={false}>
                        {
                            text ? <img style={{ height: 100 }} src={text} alt="avatar" />
                                : <div className="ant-upload-text">上传</div>
                        }
                    </Upload>
                </Fragment>
            ),
        },
        {
            title: '操作',
            key: 'operation',
            align: 'center',
            width: 64,
            fixed: 'right',
            render: (text, record) => (
                <Fragment>
                    <Button.Group>
                        <Button onClick={() => this.handleRemove(record)} icon="close" type="danger" size="small"></Button>
                    </Button.Group>
                </Fragment>
            )
        },
    ];

    componentDidMount() {
        this.handleInit();
    }

    handleUploadMain = (file, record) => {
        const { dispatch } = this.props;
        const formData = new FormData();
        formData.append('file', file);
        new Promise(resolve => {
            dispatch({
                type: 'upload/uploadImage',
                payload: {
                    resolve,
                    params: formData,
                },
            });
        }).then(res => {
            if (res.success === true) {
                let index = this.state.images.indexOf(record);
                if (index >= 0) {
                    let list = this.state.images.slice();
                    list.splice(index, 1);
                    record.imageId = res.data.id;
                    record.imageUrl = res.data.url;
                    list.splice(index, 0, record);
                    this.setState({ images: list });
                }
            } else {
                notification.error({ message: res.message });
            }
        });
    };


    handleInitGet = () => {
        const { dispatch } = this.props;
        this.setState({ loading: true });
        new Promise(resolve => {
            dispatch({
                type: 'widget/getWidgetCarousel',
                payload: {
                    resolve,
                    params: { id: this.state.id }
                },
            });
        }).then(res => {
            this.setState({ loading: false });
            if (res.success === true) {
                this.setState({
                    data: res.data,
                    images: res.data.items
                });
            } else {
                notification.error({ message: res.message });
            }
        });
    }

    handleInit = () => {
        if (this.state.id)
            this.handleInitGet();
    }

    handleSubmit = e => {
        const { dispatch, form } = this.props;
        e.preventDefault();
        form.validateFieldsAndScroll((err, values) => {
            if (!err) {
                this.setState({ submitting: true });
                var params = {
                    id: this.state.id,
                    items: this.state.images,
                    ...values
                };
                let type = 'widget/addWidgetCarousel';
                if (this.state.id)
                    type = 'widget/editWidgetCarousel';
                new Promise(resolve => {
                    dispatch({
                        type: type,
                        payload: {
                            resolve,
                            params
                        },
                    });
                }).then(res => {
                    this.setState({ submitting: false });
                    if (res.success === true) {
                        router.push('./list');
                    } else {
                        notification.error({ message: res.message, });
                    }
                });
            }
        });
    };


    handleAdd = () => {
        this.setState({ imagesLoading: true });
        let pro = {
            id: 0,
            caption: '',
            subCaption: '',
            linkText: '',
            targetUrl: '',
            imageUrl: '',
            imageId: null
        };
        this.setState({
            imagesLoading: false,
            images: [...this.state.images, pro],
        });
    }

    handleRemove = (record) => {
        this.setState(({ images }) => {
            let index = images.indexOf(record);
            let list = images.slice();
            list.splice(index, 1);
            return {
                images: list,
            };
        });
    }

    render() {
        const { form: { getFieldDecorator, getFieldValue }, } = this.props;
        const formItemLayout = {
            labelCol: {
                xs: { span: 24 },
                sm: { span: 4 },
            },
            wrapperCol: {
                xs: { span: 24 },
                sm: { span: 12 },
                md: { span: 20 },
            },
        };
        const action = (
            <Fragment>
                <Button
                    onClick={this.handleSubmit}
                    icon="save" type="primary" htmlType="submit"
                    loading={this.state.submitting}>保存</Button>
                <Link to="./list">
                    <Button>
                        <Icon type="rollback" />
                    </Button>
                </Link>
            </Fragment>
        );
        return (
            <PageHeaderWrapper title="轮播部件" action={action}>
                <Spin spinning={this.state.loading}>
                    <Card bordered={false}>
                        <Form onSubmit={this.handleSubmit}>
                            <FormItem
                                {...formItemLayout}
                                label={<span>部件名称</span>}>
                                {getFieldDecorator('name', {
                                    initialValue: this.state.data.name,
                                    rules: [{ required: true, message: '请输入部件名称' }],
                                })(<Input placeholder="部件名称" />)}
                            </FormItem>
                            <FormItem
                                {...formItemLayout}
                                label={<span>部件区域</span>}>
                                {getFieldDecorator('widgetZoneId', {
                                    rules: [{ required: true, message: '请选择部件区域' }],
                                    initialValue: this.state.data.widgetZoneId || '', valuePropName: 'value'
                                })(<Select allowClear={true}>
                                    {WidgetZoneWithId.map(c => <Option value={c.key} key={c.key}>{c.desc}</Option>)}
                                </Select>)}
                            </FormItem>
                            <FormItem
                                {...formItemLayout}
                                label={<span>发布开始时间</span>}>
                                {getFieldDecorator('publishStart', {
                                    initialValue: this.state.data.publishStart
                                        ? moment(this.state.data.publishStart, 'YYYY-MM-DD HH:mm:ss')
                                        : null
                                })(<DatePicker showTime
                                    format='YYYY-MM-DD HH:mm:ss'
                                    placeholder="发布开始时间" />)}
                            </FormItem>
                            <FormItem
                                {...formItemLayout}
                                label={<span>发布结束时间</span>}>
                                {getFieldDecorator('publishEnd', {
                                    initialValue: this.state.data.publishEnd
                                        ? moment(this.state.data.publishEnd, 'YYYY-MM-DD HH:mm:ss')
                                        : null
                                })(<DatePicker showTime
                                    format='YYYY-MM-DD HH:mm:ss'
                                    placeholder="发布结束时间" />)}
                            </FormItem>
                            <FormItem
                                {...formItemLayout}
                                label={<span>显示顺序</span>}>
                                {
                                    getFieldDecorator('displayOrder', { initialValue: this.state.data.displayOrder })(
                                        <InputNumber placeholder="显示顺序" />
                                    )
                                }
                            </FormItem>
                            <FormItem
                                {...formItemLayout}
                                label={<span>图片</span>}>
                                <Button icon="plus" type="primary" style={{ marginBottom: 16 }} onClick={this.handleAdd}>添加</Button>
                                <Table bordered={false}
                                    rowKey={(record, index) => `image_${record.id}_i_${index}`}
                                    pagination={false}
                                    loading={this.state.imagesLoading}
                                    dataSource={this.state.images}
                                    columns={this.columnsImage}
                                    scroll={{ x: 960 }}
                                />
                            </FormItem>
                        </Form>
                    </Card>
                </Spin>
            </PageHeaderWrapper>
        );
    }
}

export default WidgetCarousel;