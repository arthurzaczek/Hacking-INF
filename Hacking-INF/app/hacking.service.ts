import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';

import { Course, Example, Test, User } from './models';

import 'rxjs/add/operator/toPromise';
import { Observable } from 'rxjs/Rx';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';

@Injectable()
export class HackingService {
    private _baseUrl = 'api/';
    private _user: User = <User>{};

    // Observable navItem source
    private _userLoginSource = new BehaviorSubject<User>(this._user);
    userLoginEvent = this._userLoginSource.asObservable();

    get user(): User {
        return this._user;
    }
    
    constructor(private http: Http) {
        this._user.Name = "Anonymous";
    }

    login(user: User): Observable<User> {
        var self = this;
        self._user.IsAuthenticated = false;
        return this.http.post(this._baseUrl + 'Account/Login', user)
            .catch(error => {
                console.log(error);
                return Observable.throw(error);
            })
            .map(response => {
                self._user = response.json() as User;
                if (self._user == null) {
                    self._user = new User();
                } else {
                    self._user.IsAuthenticated = true;
                }
                self._userLoginSource.next(self._user);
                return self._user;
            });
    }

    getCourses(): Promise<Course[]> {
        return this.http.get(this._baseUrl + 'Info/GetCourses')
            .toPromise()
            .then(response => response.json() as Course[]);
    }

    getCourse(name: string): Promise<Course> {
        return this.http.get(this._baseUrl + 'Info/GetCourse?name=' + name)
            .toPromise()
            .then(response => response.json() as Course);
    }

    getExamples(course: string): Promise<Example[]> {
        return this.http.get(this._baseUrl + 'Info/GetExamples?course=' + course)
            .toPromise()
            .then(response => response.json() as Example[]);
    }

    getExample(course: string, name: string): Promise<Example> {
        return this.http.get(this._baseUrl + 'Info/GetExample?course=' + course + '&name=' + name)
            .toPromise()
            .then(response => response.json() as Example);
    }

    callCompileOrTest(course: string, example: string, sessionID: string, code: string, compileAndTest: boolean): Observable<Test> {
        var data = new Test();
        data.Course = course;
        data.Example = example;
        data.SessionID = sessionID;
        data.Code = code;
        data.CompileAndTest = compileAndTest;
        return this.http.post(this._baseUrl + 'Test', data)
            .map(response => response.json() as Test);
    }

    compile(course: string, example: string, sessionID: string, code: string): Observable<Test> {
        return this.callCompileOrTest(course, example, sessionID, code, false);
    }

    test(course: string, example: string, sessionID: string, code: string): Observable<Test> {
        return this.callCompileOrTest(course, example, sessionID, code, true);
    }

    getTestResult(sessionID: string): Observable<Test> {
        return this.http.get(this._baseUrl + 'Test/GetTestResult?sessionID=' + sessionID)
            .map(response => response.json() as Test);
    }
}