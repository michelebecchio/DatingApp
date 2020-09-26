import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-value',
  templateUrl: './value.component.html',
  styleUrls: ['./value.component.css']
})
export class ValueComponent implements OnInit {
  values: any;
  prova: string;

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.getValues();
  }

  getValues(): void {
    this.http.get('http://localhost:5000/api/values').subscribe(respnose => {
      this.values = respnose;
    }, error => {
      console.log(error);
    });
  }
}
