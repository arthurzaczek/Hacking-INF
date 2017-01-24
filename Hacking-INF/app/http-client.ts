import { Injectable } from '@angular/core';
import { Http, Headers } from '@angular/http';

@Injectable()
export class HttpClient {
    _jwt: string = null;

    constructor(private http: Http) {
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

    private createAuthorizationHeader(headers: Headers) {
        if (this._jwt != null) {
            headers.append('Authorization', 'Bearer ' + this._jwt);
        };
    }

    get(url: string) {
        let headers = new Headers();
        this.createAuthorizationHeader(headers);

        return this.http.get(url, {
            headers: headers
        });
    }

    post(url: string, data: any) {
        let headers = new Headers();
        this.createAuthorizationHeader(headers);

        return this.http.post(url, data, {
            headers: headers
        });
    }
}