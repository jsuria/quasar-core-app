<template>
  <q-page padding class="fixed-center">
    <div class="row">
      <div class="col">
        <h2>{{ $t('message.register') }}</h2>
      </div>
    </div>
    <div class="row q-my-md form-input">
      <div class="col">
        <q-input 
            v-model="email" 
            :label="$t('message.email')" 
            stack-label />
      </div>
    </div>
    <div class="row q-my-md form-input">
      <div class="col">
        <q-input 
            v-model="password" 
            :type="showPassword ? 'text' : 'password'" 
            :label="$t('message.password')" 
            stack-label>
          <template v-slot:append>
            <q-icon 
            :name="showPassword ? 'visibility' : 'visibility_off'" 
            class="cursor-pointer" 
            @click="showPassword = !showPassword" />
          </template>
        </q-input>
      </div>
    </div>
    <div class="row q-my-md form-input">
      <div class="col">
        <q-input 
            v-model="displayName" 
            :label="$t('message.displayName')" 
            stack-label />
      </div>
    </div>
    <div class="row q-my-md">
      <div class="col">
        <q-btn @click="register" color="primary" :label="$t('message.register')" :disabled="!canSend"></q-btn>
      </div>
      <div class="col">
        <q-btn flat :label="$t('message.login')" @click="$router.replace({name: 'login'})"></q-btn>
      </div>
    </div>
  </q-page>
</template>

<style scoped>
   .form-input{
     width: 300px;
   }
</style>

<script>
import api from '../common/api'
import routeCheck from '../common/route-check';
export default {
  name: 'PageRegister',
  data(){
    return {
      email: '',
      password: '',
      displayName: '',
      showPassword: false
    }
  },
  computed:{
    canSend(){
      return this.email && this.password && this.displayName;
    }
  },
  methods:{
    async register(){
      var response = await api.register(this.email, this.password, this.displayName);
      if(response.status == 200){
        this.$emit('loginSuccess', response.data.accessToken);
        if(routeCheck.validate('/index')){ 
          this.$router.replace({name: 'index'});
        }
      }
    }
  }
}
</script>