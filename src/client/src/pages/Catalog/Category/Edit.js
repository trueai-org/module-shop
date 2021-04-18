import React, { PureComponent, Fragment } from 'react';
import { connect } from 'dva';
import {
    Form, Input, Select, Button, Card, InputNumber,
    Icon, Checkbox, Upload, Modal, notification
} from 'antd';
import PageHeaderWrapper from '@/components/PageHeaderWrapper';
import router from 'umi/router';
import Link from 'umi/link';

const FormItem = Form.Item;
const { TextArea } = Input;
const Option = Select.Option;

const action = (
    <Fragment>
        <Link to="./list">
            <Button>
                <Icon type="rollback" />
            </Button>
        </Link>
    </Fragment>
);

@connect(({ category }) => ({
    category,
}))
@Form.create()
class CategoryEdit extends PureComponent {
    constructor(props) {
        super(props);
        this.state = {
            loading: false,
            categories: [],
            children: [],

            submitting: false,
            uploadLoading: false,
            mediaId: '',
            previewVisible: false,
            previewImage: '',
            fileList: [],

            _id: props.location.query.id,
            data: {}
        };
    }

    handleCancel = () => this.setState({ previewVisible: false })

    handlePreview = (file) => {
        this.setState({
            previewImage: file.url || file.thumbUrl,
            previewVisible: true,
        });
    }

    handleChange = ({ file, fileList }) => {
        // console.log(info.file.status);
        // if (info.file.status === 'uploading') {
        //     // this.setState({ uploadLoading: true });
        //     // return;
        // }
        // if (info.file.status === 'done') {
        // }
        // console.log(file.status);
        // console.log(fileList);
        // this.setState({ fileList });
    }

    handleUpload = file => {
        this.setState({ uploadLoading: true });

        const { dispatch } = this.props;

        const formData = new FormData();
        formData.append('file', file);

        new Promise(resolve => {
            dispatch({
                type: 'category/uploadImage',
                payload: {
                    resolve,
                    params: formData
                },
            });
        }).then(res => {
            this.setState({ uploadLoading: false });
            // console.log(res);
            // console.log(file);
            if (res.success === true) {
                file.url = res.data.url;
                this.setState({
                    uploadLoading: false,
                    mediaId: res.data.id,
                    fileList: [file]
                });
            } else {
                notification.error({
                    message: res.message,
                });
            }
        });
    }

    handleRemove = () => {
        this.setState({
            mediaId: '',
            fileList: []
        });
    }

    handleInit = () => {
        const { dispatch } = this.props;
        this.setState({
            loading: true
        });
        new Promise(resolve => {
            dispatch({
                type: 'category/categories',
                payload: {
                    resolve,
                },
            });
        }).then(res => {
            if (res.success === true) {
                this.setState({
                    loading: false,
                    categories: res.data
                }, () => {
                    var cs = [];
                    this.state.categories.forEach(c => {
                        cs.push(<Option value={c.id} key={c.id}>{c.name}</Option>);
                    });
                    this.setState({ children: cs });
                });
            } else {
                notification.error({
                    message: res.message,
                });
            }
        });

        if (this.state._id) {
            // const { dispatch } = this.props;
            new Promise(resolve => {
                dispatch({
                    type: 'category/firstCategory',
                    payload: {
                        resolve,
                        params: { id: this.state._id }
                    },
                });
            }).then(res => {
                if (res.success === true) {
                    if (res.data == null) {
                        notification.error({
                            message: '单据不存在',
                        });
                    } else {
                        let fs = [];
                        if (res.data.thumbnailImageUrl && res.data.mediaId) {
                            fs.push({
                                uid: '-1',
                                name: '',
                                status: 'done',
                                url: res.data.thumbnailImageUrl,
                            });
                        }
                        this.setState({
                            data: res.data,
                            mediaId: res.data.mediaId,
                            fileList: fs
                        });
                    }
                } else {
                    notification.error({
                        message: res.message,
                    });
                }
            });
        } else {
            notification.error({
                message: '参数异常',
            });
        }
    }

    componentDidMount() {
        this.handleInit();
    }

    handleSubmit = e => {
        const { dispatch, form } = this.props;
        e.preventDefault();
        form.validateFieldsAndScroll((err, values) => {
            if (!err) {
                this.setState({ submitting: true });

                var params = {
                    id: this.state._id,
                    ...values
                };
                if (this.state.mediaId) {
                    params.mediaId = this.state.mediaId;
                }

                // console.log(params);
                // return;

                new Promise(resolve => {
                    dispatch({
                        type: 'category/editCategory',
                        payload: {
                            resolve,
                            params
                        },
                    });
                }).then(res => {
                    this.setState({
                        submitting: false,
                    }, () => {
                        if (res.success === true) {
                            router.push('./list');
                        } else {
                            notification.error({
                                message: res.message,
                            });
                        }
                    });
                });
            }
        });
    };

    render() {
        const { } = this.props;
        const {
            form: { getFieldDecorator, getFieldValue },
        } = this.props;

        const formItemLayout = {
            labelCol: {
                xs: { span: 24 },
                sm: { span: 7 },
            },
            wrapperCol: {
                xs: { span: 24 },
                sm: { span: 12 },
                md: { span: 10 },
            },
        };

        const submitFormLayout = {
            wrapperCol: {
                xs: { span: 24, offset: 0 },
                sm: { span: 10, offset: 7 },
            },
        };

        const { previewVisible, previewImage, fileList } = this.state;
        const uploadButton = (
            <div>
                <Icon type={this.state.uploadLoading ? 'loading' : 'plus'} />
                <div className="ant-upload-text">上传</div>
            </div>
        );

        return (
            <PageHeaderWrapper title="商品分类 - 修改" action={action}>
                <Card bordered={false}>
                    <Form onSubmit={this.handleSubmit} hideRequiredMark style={{ marginTop: 8 }}>
                        <FormItem
                            {...formItemLayout}
                            label={<span>名称</span>}>
                            {getFieldDecorator('name', {
                                initialValue: this.state.data.name,
                                rules: [{ required: true, message: '请输入分类名称' }],
                            })(<Input placeholder="名称" />)}
                        </FormItem>
                        <FormItem
                            {...formItemLayout}
                            label={<span>父级类别</span>}>
                            {getFieldDecorator('parentId', { initialValue: this.state.data.parentId || '', valuePropName: 'value' })(
                                <Select loading={this.state.loading} allowClear={true}>
                                    {this.state.children}
                                </Select>)}
                        </FormItem>
                        <FormItem
                            {...formItemLayout}
                            label={<span>Slug</span>}>
                            {getFieldDecorator('slug', { initialValue: this.state.data.slug })(
                                <Input placeholder="Slug" />
                            )}
                        </FormItem>
                        <FormItem
                            {...formItemLayout}
                            label={<span>Meta Title</span>}>
                            {getFieldDecorator('metaTitle', { initialValue: this.state.data.metaTitle })(
                                <Input placeholder="Meta Title" />
                            )}
                        </FormItem>
                        <FormItem
                            {...formItemLayout}
                            label={<span>Meta Keywords</span>}>
                            {getFieldDecorator('metaKeywords', { initialValue: this.state.data.metaKeywords })(
                                <TextArea
                                    style={{ minHeight: 32 }}
                                    placeholder="Meta Keywords"
                                    rows={2} />
                            )}
                        </FormItem>
                        <FormItem
                            {...formItemLayout}
                            label={<span>Meta Description</span>}>
                            {getFieldDecorator('metaDescription', { initialValue: this.state.data.metaDescription })(
                                <TextArea
                                    style={{ minHeight: 32 }}
                                    placeholder="Meta Description"
                                    rows={2} />)
                            }
                        </FormItem>
                        <FormItem
                            {...formItemLayout}
                            label={<span>描述</span>}>
                            {getFieldDecorator('description', { initialValue: this.state.data.description })(
                                <TextArea
                                    style={{ minHeight: 32 }}
                                    placeholder="描述"
                                    rows={2} />
                            )}
                        </FormItem>
                        <FormItem
                            {...formItemLayout}
                            label={<span>图片</span>}>
                            <Upload action={this.handleUpload}
                                listType="picture-card"
                                fileList={fileList}
                                onRemove={this.handleRemove}
                                onPreview={this.handlePreview}
                                onChange={this.handleChange}>
                                {fileList.length >= 1 ? null : uploadButton}
                            </Upload>
                            <Modal visible={previewVisible} footer={null} onCancel={this.handleCancel}>
                                <img alt="example" style={{ width: '100%' }} src={previewImage} />
                            </Modal>
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
                            label={<span>已发布</span>}>
                            {
                                getFieldDecorator('isPublished', { initialValue: this.state.data.isPublished || false, valuePropName: 'checked' })(
                                    <Checkbox />
                                )
                            }
                        </FormItem>
                        <FormItem
                            {...formItemLayout}
                            label={<span>菜单中显示</span>}>
                            {
                                getFieldDecorator('includeInMenu', { initialValue: this.state.data.includeInMenu || false, valuePropName: 'checked' })(
                                    <Checkbox />
                                )
                            }
                        </FormItem>
                        <FormItem {...submitFormLayout} style={{ marginTop: 32 }}>
                            <Button type="primary" htmlType="submit" loading={this.state.submitting}>
                                保存
                            </Button>
                        </FormItem>
                    </Form>
                </Card>
            </PageHeaderWrapper>
        );
    }
}

export default CategoryEdit;
