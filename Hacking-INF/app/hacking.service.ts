import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';

import { Course, Example, Test, User, Category } from './models';

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

    logout(): void {
        var self = this;
        this.http.post(this._baseUrl + 'Account/Logout', { logout: true })
            .catch(error => {
                console.log(error);
                return Observable.throw(error);
            })
            .map(response => {
                self._user = new User();
                self._user.IsAuthenticated = false;
                self._userLoginSource.next(self._user);
                return self._user;
            })
            .subscribe();
    }

    whoAmI(): void {
        var self = this;
        this.http.get(this._baseUrl + 'Account/WhoAmI')
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
            })
            .subscribe();
    }

    getUserDetails(user: User): Observable<User> {
        if (user == null) {
            user = this.user;
        }
        return this.http.get(this._baseUrl + 'User?uid=' + user.UID)
            .map(response => response.json() as User);
    }

    getCourses(): Observable<Course[]> {
        return this.http.get(this._baseUrl + 'Info/GetCourses')
            .map(response => response.json() as Course[]);
    }

    getCourse(name: string): Promise<Course> {
        return this.http.get(this._baseUrl + 'Info/GetCourse?name=' + name)
            .toPromise()
            .then(response => response.json() as Course);
    }

    getCategories(course: string): Promise<Category[]> {
        return this.http.get(this._baseUrl + 'Info/GetCategories?course=' + course)
            .toPromise()
            .then(response => response.json() as Category[]);
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

    callCompileOrTest(course: string, example: string, sessionID: string, startTime: Date, code: string, compileAndTest: boolean): Observable<Test> {
        var data = new Test();
        data.Course = course;
        data.Example = example;
        data.SessionID = sessionID;
        data.StartTime = startTime;
        data.Code = code;
        data.CompileAndTest = compileAndTest;
        return this.http.post(this._baseUrl + 'Test', data)
            .map(response => response.json() as Test);
    }

    compile(course: string, example: string, sessionID: string, startTime: Date, code: string): Observable<Test> {
        return this.callCompileOrTest(course, example, sessionID, startTime, code, false);
    }

    test(course: string, example: string, sessionID: string, startTime: Date, code: string): Observable<Test> {
        return this.callCompileOrTest(course, example, sessionID, startTime, code, true);
    }

    getTestResult(sessionID: string): Observable<Test> {
        return this.http.get(this._baseUrl + 'Test/GetTestResult?sessionID=' + sessionID)
            .map(response => response.json() as Test);
    }
}