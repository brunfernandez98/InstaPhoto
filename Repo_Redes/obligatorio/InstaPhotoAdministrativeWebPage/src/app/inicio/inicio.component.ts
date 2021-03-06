import { Component, OnInit } from '@angular/core';
import { LogsService } from '../logs.service';
import { AdminService } from '../admin.service';
import { LogModel } from '../models/Log';
import { Router } from "@angular/router";
import {forEachComment} from "tslint";
import {delay} from "rxjs/operators";
import {User} from "../models/User";

@Component({
  selector: 'app-inicio',
  templateUrl: './inicio.component.html',
  styleUrls: ['./inicio.component.css']
})
export class InicioComponent implements OnInit {

  constructor(private LogsService: LogsService,private AdminService: AdminService) { }
  dataSource = [];
  selected = "";
  on = false;
  Nombre = "dsadas";
  Mail = "dsada";
  Password = "dsadsa";
  Apellido = "dsad";

  displayedColumns: string[] = ['id', 'nombre', 'email', 'password'];
  ngOnInit(): void {
    this.LogsService.getLogs().subscribe(
      ((data: Array<LogModel>) => this.resultLog(data)),
      ((error: any) => alert("errorrrrrrrrrrrrrrrr"+error.message))
    );
  }

  public onoffChange():void{
    this.on = !this.on;
    if (this.on && this.selected!=null){
      this.dataSource=this.dataSource.filter((item: any) =>item.typeSeverity==this.selected)
    }
    if(!this.on){
      this.LogsService.getLogs().subscribe(
        ((data: Array<LogModel>) => this.resultLog(data)),
        ((error: any) => alert("error "+error.error))
      );
    }

  }

  public addUser():void{
    if (this.Nombre!=null&& this.Apellido!=null&& this.Mail!=null&& this.Password!=null){
      var user =  new User(this.Nombre,this.Apellido,this.Mail,this.Password);
      console.log(this.Password);
      console.log(this.Mail);
      console.log(this.Apellido);
      console.log(this.Nombre);
      this.AdminService.postUser(user).subscribe(
        ((data: any) => alert(data)),
        ((error: any) => alert("error" + error.message))
      );
    }

  }
  public resultLog(data:Array<LogModel>):void{
    this.dataSource = data;
    this.dataSource.forEach(value => {
      switch (value.typeSeverity){
        case 0:
          value.typeSeverity = "INFO"
          break;
        case 1:
          value.typeSeverity = "WARNING"
          break;
        case 2:
          value.typeSeverity = "ERROR"
          break;
      }
      switch (value.type){
        case 0:
          value.type = "LOGINUSER"
          break;
        case 1:
          value.type = "UPLOADPHOTO"
          break;
        case 2:
          value.type = "CREATEUSER"
          break;
        case 3:
          value.type = "COMMENTONEPHOTO"
          break;
        case 4:
          value.type = "SEEALLUSER"
          break;
        case 5:
          value.type = "SEECOMMENT"
          break;
        case 6:
          value.type = "SIGNOUT"
          break;
        case 7:
          value.type = "SEEPHOTOONEUSER"
          break;
        case 8:
          value.type = "DELETEUSER"
          break;
        case 9:
          value.type = "MODIFYUSERNAME"
          break;
        case 10:
          value.type = "MODIFYUSERPASSWORD"
          break;
        case 11:
          value.type = "MODIFYUSERLASTNAME"
          break;
        case 12:
          value.type = "MODIFYUSEREMAIL"
          break;


      }
    })
  }


}
