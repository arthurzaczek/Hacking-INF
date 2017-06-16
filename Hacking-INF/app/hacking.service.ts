import { Injectable } from '@angular/core';
import { HttpClient } from './http-client';

import { Course, Example, Test, User, Category, ExampleResult, ExampleStat, StudentStat, CompilerMessage, ReportedCompilerMessage, LogLineModel } from './models';

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

    constructor(private http: HttpClient) {
        this._user.Name = "Anonymous";
        let token = localStorage.getItem("__jwt__");
        this.http.setJwt(token);
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
                    this.http.setJwt(null);
                    localStorage.removeItem("__jwt__");
                } else {
                    self._user.IsAuthenticated = true;
                    this.http.setJwt(self._user.Jwt);
                    localStorage.setItem("__jwt__", self._user.Jwt);
                }
                self._userLoginSource.next(self._user);
                return self._user;
            });
    }

    logout(): void {
        this.http.setJwt(null);
        localStorage.removeItem("__jwt__");
        this._user = new User();
        this._user.IsAuthenticated = false;
        this._userLoginSource.next(this._user);
    }

    whoAmI(): void {
        if (!this.http.hasJwt()) return;

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
                    this.http.setJwt(null);
                    localStorage.removeItem("__jwt__");
                } else {
                    self._user.IsAuthenticated = true;
                    this.http.setJwt(self._user.Jwt);
                    localStorage.setItem("__jwt__", self._user.Jwt);
                }
                self._userLoginSource.next(self._user);
                return self._user;
            })
            .subscribe();
    }

    getAccessToken(): Observable<string> {
        if (!this.http.hasJwt()) return Observable.throw("Not logged in");

        return this.http.get(this._baseUrl + 'Account/GetToken')
            .map(response => response.json() as string);
    }

    getUserDetails(user: User): Observable<User> {
        if (user == null) {
            user = this.user;
        }
        return this.http.get(this._baseUrl + 'User?uid=' + user.UID)
            .map(response => response.json() as User);
    }

    getAdminStats(): Observable<ExampleStat[]> {
        return this.http.get(this._baseUrl + 'Admin/GetStats')
            .map(response => response.json() as ExampleStat[]);
    }

    getAdminStatsStudents(course: string, year: number): Observable<StudentStat[]> {
        return this.http.get(this._baseUrl + 'Admin/GetStatsStudents?course=' + course + '&year=' + year)
            .map(response => response.json() as StudentStat[]);
    }
    getAdminStatsStudentsCSVUrl(course: string, year: number): string {
        return this._baseUrl + 'Admin/GetStatsStudentsCSV?course=' + encodeURIComponent(course) + '&year=' + encodeURIComponent((year || 0).toString());
    }
    getAdminStatsStudentDetailsCSVUrl(course: string, year: number): string {
        return this._baseUrl + 'Admin/GetStatsStudentDetailsCSV?course=' + encodeURIComponent(course) + '&year=' + encodeURIComponent((year || 0).toString());
    }

    getAdminDownloadUrl(course: string, example: string) {
        return this._baseUrl + 'Admin/Download?course=' + encodeURIComponent(course) + '&example=' + encodeURIComponent(example);
    }

    getAdminLogfile(type: string): Observable<LogLineModel[]> {
        return this.http.get(this._baseUrl + 'Admin/GetLogfile?type=' + type)
            .map(response => response.json() as LogLineModel[]);
    }

    getAdminReportedCompilerMessages(): Observable<ReportedCompilerMessage[]> {
        return this.http.get(this._baseUrl + 'Admin/GetReportedCompilerMessages')
            .map(response => response.json() as ReportedCompilerMessage[]);
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

    getCompilerMessages(): Promise<CompilerMessage[]> {
        // TODO: Cache them
        return this.http.get(this._baseUrl + 'Info/GetCompilerMessages')
            .toPromise()
            .then(response => response.json() as CompilerMessage[]);
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