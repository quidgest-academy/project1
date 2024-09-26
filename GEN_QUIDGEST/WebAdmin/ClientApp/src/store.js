import { createStore } from 'vuex'

const store = createStore({
  state: {
    currentApp: '',
    currentYear: '',
    currentLanguage: ''
  },
  mutations: {
    SET_APP: function (state, newValue) {
      state.currentApp = newValue;
    },
    SET_YEAR: function (state, newValue) {
      state.currentYear = newValue;
    },
    SET_LANGUAGE: function (state, newValue) {
      state.currentLanguage = newValue;
    }
  },
  actions: {
    changeApp: function (context, newValue) {
        context.commit("SET_APP", newValue);
    },
    changeYear: function (context, newValue) {
        context.commit("SET_YEAR", newValue);
    },
    changeLanguage: function (context, newValue) {
        context.commit("SET_LANGUAGE", newValue);
    }
  },
  getters: {
    App: state => state.currentApp,
    Year: state => state.currentYear,
    Language: state => state.currentLanguage
  }
});

export default store;