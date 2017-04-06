import * as branchService from '../../api/branchService';

const branch = {
    name: "",
    isRemote: false,
    isHead: false
}

const state = {
    branchList: [],
    currentBranch: {}
}

const mutations = {
    SET_ALL_BRANCHES(state, branchList) {
        state.branchList = branchList;
    }
}

const getters = {
  currentBranch: state => {
    return state.branchList.filter(s => s.isHead)[0];
  }
}

const actions = {
        GET_ALL_BRANCHES({
            commit
        }) {
            branchService.getAllbranches()
                .then((b) => commit('SET_ALL_BRANCHES', b))
        },
        CHECKOUT_BRANCH({commit},branchName){
          return branchService.checkoutBranch(branchName)
        },
        CREATE_NEW_BRANCH({dispatch,commit}, branchName) {
            return new Promise((resolve, reject) => {
                    branchService.createNewBranch(branchName)
                        .then((b) => {
                          dispatch('CHECKOUT_BRANCH', branchName)
                          .then(() => dispatch('GET_ALL_BRANCHES'))
                        })
                        .then(resolve());
                })
            }
        }

export default {
    state,
    mutations,
    actions,
    getters
}
