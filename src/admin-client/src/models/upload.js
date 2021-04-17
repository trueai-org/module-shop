import { uploadImage,uploadMultipleImage } from '@/services/api';

export default {
    namespace: 'upload',
    state: {
        media: {},
        mediaList: []
    },

    effects: {
        *uploadImage2({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(uploadImage, params);

            if (!response) {
                return;
            }
            if (response.success === true) {
                yield put({
                    type: 'saveMedia',
                    payload: response.data,
                });
            }

            // !!resolve && resolve(response);
            // console.log(response);
            // return response;
        },

        *uploadMultipleImage({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(uploadMultipleImage, params);
            !!resolve && resolve(response);
        },

        *uploadImage({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(uploadImage, params);
            !!resolve && resolve(response);
        },
    },

    reducers: {
        saveMedia(state, { payload }) {
            console.log('payload');
            console.log(state);
            let ls = [];
            ls = state.mediaList;
            ls.push(payload);

            return {
                ...state,
                media: payload,
                mediaList: ls
            };
        },
    },
};