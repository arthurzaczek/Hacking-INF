import { Injectable } from '@angular/core';
import { Headers, Http, Response } from '@angular/http';

import { Course, Example, Test } from './models';

import 'rxjs/add/operator/toPromise';
import { Observable } from 'rxjs/Observable';

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

    callCompileOrTest(course: string, example: string, sessionID: string, code: string, compileAndTest: boolean): Observable<Response> {
        var data = new Test();
        data.Course = course;
        data.Example = example;
        data.SessionID = sessionID;
        data.Code = code;
        data.CompileAndTest = compileAndTest;
        return this.http.post(this.baseUrl + 'Test', data);
    }

    compile(course: string, example: string, sessionID: string, code: string): Observable<Response> {
        return this.callCompileOrTest(course, example, sessionID, code, false);
    }

    test(course: string, example: string, sessionID: string, code: string): Observable<Response> {
        return this.callCompileOrTest(course, example, sessionID, code, true);
    }
}