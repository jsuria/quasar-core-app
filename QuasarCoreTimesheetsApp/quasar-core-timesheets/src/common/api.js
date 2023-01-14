import axios from 'axios';
import {SessionStorage} from 'quasar';

axios.interceptors.request.use(config => {
  var token = SessionStorage.getItem('access_token');
  config.headers = {
    Authorization: `Bearer ${token}`
  };
  //config.withCredentials = true

  if(process.env.PROD){
    config.baseURL = 'https://timesheets-test123.azurewebsites.net/api/';
  }
  else {
    //config.baseURL = 'https://localhost:44343/api/';
    config.baseURL = 'https://localhost:5001/api/';
  }
  return config;
});

export default {
  register(email,password,displayName){
    var data = {
      email: email,
      password: password,
      displayName: displayName
    };
    return axios.post('users/register', data);
  },
  login(email,password){
    var data = {
      email: email,
      password: password
    };
    return axios.post('users/login', data);
  },
  refreshToken(){
    return axios.post('users/refreshtoken', { withCredentials: true });
  },
  getTimesheets(startDate, endDate){
    var query = {
      startDate: startDate.toISOString(),
      endDate: endDate.toISOString()
    };
    return axios.get('timesheets', { params: query, withCredentials: true});
  },
  getAllTimesheets(startDate,endDate){
    var query = {
      startDate: startDate.toISOString(),
      endDate: endDate.toISOString()
    };
    return axios.get('timesheets/all', { params: query, withCredentials: true });
  },
  startTimesheet(){
    return axios.post('timesheets/start', {}, { withCredentials: true });
  },
  endTimesheet(id){
    return axios.put(`timesheets/${id}/end`, {}, { withCredentials: true });
  },
  createAbsence(date, comment){
    var data = {
      date: date.toISOString(),
      comment: comment
    };
    return axios.post('timesheets/absence', data, { withCredentials: true });
  },
  deleteAbsence(id){
    return axios.delete(`timesheets/absence/${id}`, { withCredentials: true });
  }
}