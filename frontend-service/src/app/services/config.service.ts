import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ConfigService {

  private _api_url = 'http://localhost:8000/api'; // ovo
 private _auth_url = 'http://localhost:8000/api/auth'; // ovo
 // private _api_url = 'http://localhost:8081/api';
  //private _tour_api_url = 'http://localhost:8082/api';
  //private _auth_url = 'http://localhost:8081/auth';
  //private _api_url = 'http://stakeholders-service:8080/api';
  //private _tour_api_url = 'http://tour-service:8080/api';
  //private _auth_url = 'http://stakeholders-service:8080/auth';
  private _user_url = this._api_url + '/user'; // ovo
  private _tours_url = this._api_url + '/tours'; // ovo
  private _reviews_url = this._api_url + '/tours';  // ovo
  
  //private _api_url = 'http://localhost:8080/api';
  //private _tour_api_url = 'http://localhost:55814/api';
  //private _auth_url = 'http://localhost:8080/auth';
  //private _user_url = this._api_url + '/user';
  //private _tours_url = this._tour_api_url + '/tours';
  //private _reviews_url = this._api_url + '/tours'; 


  get tours_url(): string {
    return this._tours_url;
  }

  get reviews_url(): string {
    return this._reviews_url;
  }

  get apiUrl(): string {
    return this._api_url;
  }

  private _login_url = this._auth_url + '/login';

  get login_url(): string {
    return this._login_url;
  }

  private _whoami_url = this._api_url + '/getMyInfo';

  get whoami_url(): string {
    return this._whoami_url;
  }

  private _users_url = this._user_url + '/all';

  get users_url(): string {
    return this._users_url;
  }

  private _signup_url = this._auth_url + '/signup';

  get signup_url(): string {
    return this._signup_url;
  }

}