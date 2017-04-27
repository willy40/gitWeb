import * as repositoryApi from '../../api/repository';

const state = {
    mappedRepositories : [],
    currentRepository : ''
}

const actions = {
    FETCH_MAPPED_REPOSITORIES({commit}) {
        repositoryApi.fetch_mapped_repository()
            .then(repositories => commit('setMappedRepositories', repositories));
    }
}

const mutations = {
    setMappedRepositories(state, repositories) {
        state.mappedRepositories = repositories;
    },
    setCurrentRepository(state,repositoryPath){
      state.currentRepository = repositoryPath;
    }
}

export default {
    state,
    mutations,
    actions
}
