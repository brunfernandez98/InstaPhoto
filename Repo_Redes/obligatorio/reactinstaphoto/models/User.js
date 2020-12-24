export class User {
nombre: String;
  apellido: String;
  email: String;
  password: String;

  constructor(nombre: String,apellido: String, email: String,password: String){
    this.nombre = nombre;
    this.apellido = apellido;
    this.email = email;
    this.password = password;
  }
}
