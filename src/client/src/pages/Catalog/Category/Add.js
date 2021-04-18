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
// const children = [];

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
class CategoryAdd extends PureComponent {
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

    componentDidMount() {
        this.setState({
            loading: true,
        });

        const { dispatch } = this.props;
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
                        cs.push(<Option key={c.id}>{c.name}</Option>);
                    });
                    this.setState({ children: cs });
                });
            } else {
                notification.error({
                    message: res.message,
                });
            }
        });
    }

    handleSubmit = e => {
        const { dispatch, form } = this.props;
        e.preventDefault();
        form.validateFieldsAndScroll((err, values) => {
            if (!err) {
                this.setState({ submitting: true });
                var params = {
                    ...values
                };
                if (this.state.mediaId) {
                    params.mediaId = this.state.mediaId;
                }

                new Promise(resolve => {
                    dispatch({
                        type: 'category/addCategory',
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
                            // dispatch(routerRedux.push('/catalog/category/list'));
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
            <PageHeaderWrapper title="商品分类 - 添加" action={action}>
                <Card bordered={false}>
                    <Form onSubmit={this.handleSubmit} hideRequiredMark style={{ marginTop: 8 }}>
                        <FormItem
                            {...formItemLayout}
                            label={<span>名称</span>}>
                            {getFieldDecorator('name', {
                                initialValue: '',
                                rules: [{ required: true, message: '请输入分类名称' }],
                            })(
                                <Input placeholder="名称" />)}
                        </FormItem>
                        <FormItem
                            {...formItemLayout}
                            label={<span>父级类别</span>}>
                            {getFieldDecorator('parentId', { initialValue: '' })(
                                <Select loading={this.state.loading} allowClear={true}>
                                    {this.state.children}
                                </Select>)}
                        </FormItem>
                        <FormItem
                            {...formItemLayout}
                            label={<span>Slug</span>}>
                            {getFieldDecorator('slug', { initialValue: '' })(
                                <Input placeholder="Slug" />
                            )}
                        </FormItem>
                        <FormItem
                            {...formItemLayout}
                            label={<span>Meta Title</span>}>
                            {getFieldDecorator('metaTitle', { initialValue: '' })(
                                <Input placeholder="Meta Title" />
                            )}
                        </FormItem>
                        <FormItem
                            {...formItemLayout}
                            label={<span>Meta Keywords</span>}>
                            {getFieldDecorator('metaKeywords', { initialValue: '' })(
                                <TextArea
                                    style={{ minHeight: 32 }}
                                    placeholder="Meta Keywords"
                                    rows={2} />
                            )}
                        </FormItem>
                        <FormItem
                            {...formItemLayout}
                            label={<span>Meta Description</span>}>
                            {getFieldDecorator('metaDescription', { initialValue: '' })(
                                <TextArea
                                    style={{ minHeight: 32 }}
                                    placeholder="Meta Description"
                                    rows={2} />)
                            }
                        </FormItem>
                        <FormItem
                            {...formItemLayout}
                            label={<span>描述</span>}>
                            {getFieldDecorator('description', { initialValue: '' })(
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
                                getFieldDecorator('displayOrder', { initialValue: '0' })(
                                    <InputNumber placeholder="显示顺序" />
                                )
                            }
                        </FormItem>
                        <FormItem
                            {...formItemLayout}
                            label={<span>已发布</span>}>
                            {
                                getFieldDecorator('isPublished', { initialValue: false })(
                                    <Checkbox defaultChecked={false} />
                                )
                            }
                        </FormItem>
                        <FormItem
                            {...formItemLayout}
                            label={<span>菜单中显示</span>}>
                            {
                                getFieldDecorator('includeInMenu', { initialValue: false })(
                                    <Checkbox defaultChecked={false} />
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

export default CategoryAdd;
