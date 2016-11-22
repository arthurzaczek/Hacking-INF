import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';

import { Course, Example } from './models';

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

    getCourse(name: string): Promise<Course> {
        return this.http.get(this.baseUrl + 'Info/GetCourse?name=' + name)
            .toPromise()
            .then(response => response.json() as Course);
    }

    getExamples(course: string): Promise<Example[]> {
        return this.http.get(this.baseUrl + 'Info/GetExamples?course=' + course)
            .toPromise()
            .then(response => response.json() as Example[]);
    }

    getExample(course: string, name: string): Promise<Example> {
        return this.http.get(this.baseUrl + 'Info/GetExample?course=' + course + '&name=' + name)
            .toPromise()
            .then(response => response.json() as Example);
    }
}