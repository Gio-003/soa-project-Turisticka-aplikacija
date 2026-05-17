import {HttpClient, HttpHeaders, HttpRequest, HttpResponse, HttpParams} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {Observable} from 'rxjs';
import {catchError, filter, map} from 'rxjs/operators';

export enum RequestMethod {
  Get = 'GET',
  Head = 'HEAD',
  Post = 'POST',
  Put = 'PUT',
  Delete = 'DELETE',
  Options = 'OPTIONS',
  Patch = 'PATCH'
}

@Injectable({
  providedIn: 'root'
})
export class ApiService {

  headers = new HttpHeaders({
    'Accept': 'application/json',
    'Content-Type': 'application/json'
  });

  constructor(private http: HttpClient) { }

  // MODIFIKUJTE 'get' METODU
  get(path: string, args?: any, responseType: 'json' | 'text' | 'blob' = 'json'): Observable<any> {
    const token = localStorage.getItem('jwt_token');
    let headers = this.headers;

    if (token) {
      headers = headers.set('Authorization', `Bearer ${token}`);
    }

    // Prosledite responseType u options objekat
    const options = {
      headers,
      responseType: responseType as 'json' // Potrebno je kastovanje zbog tipova
    };

    return this.http.get(path, options)
      .pipe(catchError(this.checkError.bind(this)));
  }

   post(path: string, body: any, customHeaders?: HttpHeaders, responseType: 'json' | 'text' | 'blob' = 'json'): Observable<any> {
    return this.request(path, body, RequestMethod.Post, customHeaders, responseType);
  }


  put(path: string, body: any): Observable<any> {
    return this.request(path, body, RequestMethod.Put);
  }

  delete(path: string, body?: any): Observable<any> {
    return this.request(path, body, RequestMethod.Delete);
  }

  private request(
  path: string,
  body: any,
  method = RequestMethod.Post,
  custemHeaders?: HttpHeaders,
  responseType: 'json' | 'text' | 'blob' = 'json'
): Observable<any> {

  const token = localStorage.getItem('jwt_token');
  let headers = custemHeaders || this.headers;

  if (token) {
    headers = headers.set('Authorization', `Bearer ${token}`);
  }

    const req = new HttpRequest(method, path, body, {
      headers: headers,
      responseType: responseType
    });

  return this.http.request(req).pipe(
    filter(
      (response): response is HttpResponse<any> =>
        response instanceof HttpResponse
    ),
    map(response => response.body),
    catchError(error => this.checkError(error))
  );
}

  private checkError(error: any): any {
    throw error;
  }

  private serialize(obj: any): HttpParams {
    let params = new HttpParams();
  
    for (const key in obj) {
      if (obj.hasOwnProperty(key) && !this.looseInvalid(obj[key])) {
        params = params.set(key, obj[key]);
      }
    }
  
    return params;
  }

  private looseInvalid(a: string | number): boolean {
    return a === '' || a === null || a === undefined;
  }

}