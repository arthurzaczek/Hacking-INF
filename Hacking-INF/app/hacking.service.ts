
import {throwError as observableThrowError,  Subject, Observable, Subscription } from 'rxjs';
import { map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HackingHttpClient } from './hacking-http-client';

import { Course, Example, Test, User, Category, ExampleResult, ExampleStat, StudentStat, CompilerMessage, ReportedCompilerMessage, LogLineModel } from './models';

@Injectable()
export class HackingService {
    private _baseUrl = 'api/';
    private _user: User = new User();

    // Observable navItem source
    public userLoginEvent = new Subject<User>();

    get user(): User {
        return this._user;
    }

    constructor(private http: HackingHttpClient) {
        this._user.Name = "Anonymous";
        let token = localStorage.getItem("__jwt__");
        this.http.setJwt(token);
    }

    login(user: User): Observable<User> {
        var self = this;
        self._user.IsAuthenticated = false;
        return this.http.post<User>(this._baseUrl + 'Account/Login', user).pipe(
            map(response => {
                self._user = response;
                if (self._user == null) {
                    self._user = new User();
                    this.http.setJwt(null);
                    localStorage.removeItem("__jwt__");
                } else {
                    self._user.IsAuthenticated = true;
                    this.http.setJwt(self._user.Jwt);
                    localStorage.setItem("__jwt__", self._user.Jwt);
                }
                self.userLoginEvent.next(self._user);
                return self._user;
            }));
    }

    logout(): void {
        this.http.setJwt(null);
        localStorage.removeItem("__jwt__");
        this._user = new User();
        this._user.IsAuthenticated = false;
        this.userLoginEvent.next(this._user);
    }

    whoAmI(): void {
        if (!this.http.hasJwt()) return;

        var self = this;
        this.http.get(this._baseUrl + 'Account/WhoAmI').pipe(
            map(response => {
                self._user = response as User;
                if (self._user == null) {
                    self._user = new User();
                    this.http.setJwt(null);
                    localStorage.removeItem("__jwt__");
                } else {
                    self._user.IsAuthenticated = true;
                    this.http.setJwt(self._user.Jwt);
                    localStorage.setItem("__jwt__", self._user.Jwt);
                }
                self.userLoginEvent.next(self._user);
                return self._user;
            }));
    }

    getAccessToken(): Observable<string> {
        if (!this.http.hasJwt()) return observableThrowError("Not logged in");

        return this.http.get<string>(this._baseUrl + 'Account/GetToken').pipe(
            map(response => {
                return response;
            }));
    }

    getUserDetails(user: User): Observable<User> {
        if (user == null) {
            user = this.user;
        }
        return this.http.get<User>(this._baseUrl + 'User?uid=' + user.UID).pipe(
            map(response => {
                return response;
            }));
    }

    getAdminStats(): Observable<ExampleStat[]> {
        return this.http.get<ExampleStat[]>(this._baseUrl + 'Admin/GetStats').pipe(
            map(response => {
                return response;
            }));
    }

    getAdminStatsStudents(course: string, year: number): Observable<StudentStat[]> {
        return this.http.get<StudentStat[]>(this._baseUrl + 'Admin/GetStatsStudents?course=' + course + '&year=' + year).pipe(
            map(response => {
                return response;
            }));
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
        return this.http.get<LogLineModel[]>(this._baseUrl + 'Admin/GetLogfile?type=' + type).pipe(
            map(response => {
                return response;
            }));
    }

    getAdminReportedCompilerMessages(): Observable<ReportedCompilerMessage[]> {
        return this.http.get<ReportedCompilerMessage[]>(this._baseUrl + 'Admin/GetReportedCompilerMessages').pipe(
            map(response => {
                return response;
            }));
    }

    updateExamples(): Observable<string> {
        return this.http.post<string>(this._baseUrl + 'Admin/UpdateExamples', null).pipe(
            map(response => {
                return response;
            }));
    }

    clearCache(): Observable<string> {
        return this.http.post<string>(this._baseUrl + 'Admin/ClearCache', null).pipe(
            map(response => {
                return response;
            }));
    }
    loadTest(concurrent: number, numberOfTests: number): Observable<string> {
        return this.http.post<string>(this._baseUrl + 'Admin/LoadTest?concurrent=' + concurrent + '&numberOfTests=' + numberOfTests, null).pipe(
            map(response => {
                return response;
            }));
    }

    getCourses(): Observable<Course[]> {
        return this.http.get<Course[]>(this._baseUrl + 'Info/GetCourses').pipe(
            map(response => {
                return response;
            }));
    }

    getCourse(name: string): Observable<Course> {
        return this.http.get<Course>(this._baseUrl + 'Info/GetCourse?name=' + name).pipe(
            map(response => {
                return response;
            }));
    }

    getCategories(course: string): Observable<Category[]> {
        return this.http.get<Category[]>(this._baseUrl + 'Info/GetCategories?course=' + course).pipe(
            map(response => {
                return response;
            }));
    }

    getCompilerMessages(): Observable<CompilerMessage[]> {
        // TODO: Cache them
        return this.http.get<CompilerMessage[]>(this._baseUrl + 'Info/GetCompilerMessages').pipe(
            map(response => {
                return response;
            }));
    }

    getExamples(course: string): Observable<Example[]> {
        return this.http.get<Example[]>(this._baseUrl + 'Info/GetExamples?course=' + course).pipe(
            map(response => {
                return response;
            }));
    }

    getExample(course: string, name: string): Observable<Example> {
        return this.http.get<Example>(this._baseUrl + 'Info/GetExample?course=' + course + '&name=' + name).pipe(
            map(response => {
                return response;
            }));
    }

    callCompileOrTest(course: string, example: string, sessionID: string, startTime: Date, code: string, compileAndTest: boolean): Observable<Test> {
        var data = new Test();
        data.Course = course;
        data.Example = example;
        data.SessionID = sessionID;
        data.StartTime = startTime;
        data.Code = code;
        data.CompileAndTest = compileAndTest;
        return this.http.post<Test>(this._baseUrl + 'Test', data).pipe(
            map(response => {
                return response;
            }));
    }

    compile(course: string, example: string, sessionID: string, startTime: Date, code: string): Observable<Test> {
        return this.callCompileOrTest(course, example, sessionID, startTime, code, false);
    }

    test(course: string, example: string, sessionID: string, startTime: Date, code: string): Observable<Test> {
        return this.callCompileOrTest(course, example, sessionID, startTime, code, true);
    }

    getTestResult(sessionID: string): Observable<Test> {
        return this.http.get<Test>(this._baseUrl + 'Test/GetTestResult?sessionID=' + sessionID).pipe(
            map(response => {
                return response;
            }));
    }
}
