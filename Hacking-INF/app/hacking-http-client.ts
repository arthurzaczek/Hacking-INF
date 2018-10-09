
import {throwError as observableThrowError,  Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Injectable()
export class HackingHttpClient {
    _jwt: string = null;

    constructor(private http: HttpClient) {
    }

    setJwt(token: string) {
        if (token != null && token != "") {
            this._jwt = token;
        } else {
            this._jwt = null;
        }
    }

    hasJwt(): boolean {
        return this._jwt != null;
    }

    private handleError(error: any, url: string) {
        let status = error ? error.status || -1 : -1;
        console.error("Error " + status + " calling " + url);
        observableThrowError("Error in request");
    }

    private createAuthorizationHeader(headers: HttpHeaders) {
        if (this._jwt != null) {
            headers = headers.set('Authorization', 'Bearer ' + this._jwt);
        };

        return headers;
    }

    get<T>(url: string): Observable<T> {
        let headers = new HttpHeaders();
        headers = this.createAuthorizationHeader(headers);

        let result = new Observable<T>(observer => {
            this.http.get<T>(url, {
                headers: headers
            }).subscribe(
                data => observer.next(data),
                error => this.handleError(error, url));
        });
        return result;
    }

    post<T>(url: string, data: any): Observable<T> {
        let headers = new HttpHeaders();
        headers = this.createAuthorizationHeader(headers);

        let result = new Observable<T>(observer => {
            this.http.post<T>(url, data, {
                headers: headers
            }).subscribe(
                data => observer.next(data),
                error => this.handleError(error, url));
        });
        return result;
    }
}
