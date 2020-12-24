export class LogModel {
Author: String;
DateTime: Date;
Type: Number;
TypeSeverity: Number;

  constructor(Author: String,DateTime: Date,Type: Number,TypeSeverity: Number){
    this.Author = Author;
    this.DateTime = DateTime;
    this.Type = Type;
    this.TypeSeverity = TypeSeverity;
  }
}
