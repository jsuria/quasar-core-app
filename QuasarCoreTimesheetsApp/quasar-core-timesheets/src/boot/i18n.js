import Vue from 'vue'
import VueI18n from 'vue-i18n'
import message from '../i18n/en-US/index'

Vue.use(VueI18n)

const messages = {
  en:{
      message: message
      /* message: {
        appTitle: 'Timesheets',
        myTimesheets: 'My Timesheets',
        register: 'Register',
        login: 'Login',
        logout: 'Log out',

        email: 'Email',
        password: 'Password',
        displayName: 'Display Name',
        
        absence: 'Absence',
        deleteAbsence: 'Delete absence',
        absenceReason: 'Reason',
        startTime: 'Start Time',
        endTime: 'End Time',
        flexTime: 'Flex Time',
        total: 'Total',
        start: 'Start',
        end: 'End',
        overview: 'Overview'
      }, */
  }
}


const i18n = new VueI18n({
  locale: 'en',
  fallbackLocale: 'en',
  messages
})

export default ({ app }) => {
  // Set i18n instance on app
  app.i18n = i18n
}

export { i18n }
