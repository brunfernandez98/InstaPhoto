import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {HttpClient, HttpHeaders,HttpParams} from '@angular/common/http';
import {environment} from "../environments/environment";
import {LogModel} from "./models/Log";
import {User} from "./models/User";

@Injectable({
  providedIn: 'root'
})
export class AdminService {

  private WEB_API_URL: string = environment.apiAdminsUrl;
  constructor(private _httpService: HttpClient) { }

  public posttUser(user : User , email: string , nombre: string,apellido: string,password: string): Observable<string> {
    const myHeaders = new HttpHeaders().append("nombre",nombre);
    myHeaders.append("Accept", "application/text");
    myHeaders.append('Access-Control-Allow-Headers', 'Content-Type');
    myHeaders.append("nombre",nombre);
    //myHeaders.append("email",email);
    //myHeaders.append("password",password);
    //myHeaders.append("user",user);
    const httpOptions = {
      headers: myHeaders,
    };
    return this._httpService.post<string>(
      this.WEB_API_URL + "WeatherForecast",
      httpOptions
    );
  }



  public putSolicitud(email: string , valor: string,opcion: string): Observable<string> {
    const myHeaders = new HttpHeaders().set("auth", localStorage.getItem('token'));
    myHeaders.append("Accept", "application/text");
    myHeaders.append('Access-Control-Allow-Methods: GET, POST, PUT, DELETE');

    const httpOptions = {
      headers: myHeaders,
    };
    return this._httpService.put<string>(
      this.WEB_API_URL + "WeatherForecast",
      email,
      httpOptions
    );
  }
  public postUser(email: string , nombre: string,apellido: string,password: string): Observable<string> {
      var myHeaders = new HttpHeaders();
    myHeaders =myHeaders.append("Accept", "application/json");


     var user =  new User("diego","suero","diegp@Wdd.com","passwprd");

      const httpOptions = {
        headers: myHeaders,
      };
      return this._httpService.post<string>(
        this.WEB_API_URL + "WeatherForecast",{nombre : "diegooooooo",apelldo : "suerito",email:"suerito@gmail.com",password:"password"},

        httpOptions
      );
    }
  getTest(email: string): Observable<string> {
    const myHeaders = new HttpHeaders().append("email",email);
    myHeaders.append("Accept", "application/json");
    const httpOptions = {
      headers: myHeaders,
    };

    return this._httpService.get<string>(
      this.WEB_API_URL + "WeatherForecast",
      email,
      httpOptions
    );

  }
}
