import { Injectable } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { LogModel } from "./models/Log";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Observable } from "rxjs";
import { map, tap, catchError } from 'rxjs/operators';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class LogsService {
  private WEB_API_URL: string = environment.apiLogsUrl;
  constructor(private _httpService: HttpClient) { }
  getLogs(): Observable<Array<LogModel>> {
    const myHeaders = new HttpHeaders();
    myHeaders.append("Accept", "application/json");
    const httpOptions = {
      headers: myHeaders,
    };
  console.log("entro");
    return this._httpService.get<Array<LogModel>>(
      this.WEB_API_URL + "Logs",
      httpOptions
    );
  }
}
