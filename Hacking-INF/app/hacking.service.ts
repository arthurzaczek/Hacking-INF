import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';

import { Course } from './models';
import 'rxjs/add/operator/toPromise';

@Injectable()
export class HackingService {
    private baseUrl = 'api/';

    constructor(private http: Http) { }

    getCourses(): Promise<Course[]> {
        return this.http.get(this.baseUrl + 'Info/GetCourses')
            .toPromise()
            .then(response => response.json() as Course[]);
    }
}