import { Component, Input, OnInit, AfterViewInit, ElementRef } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { Location } from '@angular/common';
import { Course, Example, Test, User } from './models';
import { HackingService } from './hacking.service';

import { Observable } from 'rxjs/Rx';

import 'rxjs/add/operator/switchMap';
import 'rxjs/add/operator/map';

declare var jsHelper: any;

@Component({
    selector: 'example-detail',
    templateUrl: 'app/example-detail.component.html'
})
export class ExampleDetailComponent implements OnInit, AfterViewInit {
    constructor(
        private hackingService: HackingService,
        private route: ActivatedRoute,
        private location: Location,
        private element: ElementRef) { }

    example: Example = <Example>{};
    course: Course = <Course>{};
    result: Test = <Test>{};
    user: User = <User>{};

    hasError: boolean = false;
    errorMessage: String = "";

    timeElapsed: string = "00:00:00";

    ngOnInit(): void {
        this.route.params
            .switchMap((params: Params) => this.hackingService.getExample(params['course'], params['name']))
            .subscribe(data => {
                this.example = data;
                if (this.hackingService.user != null)
                    this.user = this.hackingService.user;
                this.updateEditor();
            });
        this.route.params
            .switchMap((params: Params) => this.hackingService.getCourse(params['course']))
            .subscribe(data => this.course = data);

        var self = this;
        let timer = Observable.timer(1000, 1000);
        timer.subscribe(t => {
            if (!self.result.Succeeded) {
                var diff = new Date().getTime() - new Date(self.example.StartTime).getTime();

                var hours = Math.floor(diff / (1000 * 60 * 60));
                diff -= hours * (1000 * 60 * 60);

                var mins = Math.floor(diff / (1000 * 60));
                diff -= mins * (1000 * 60);

                var seconds = Math.floor(diff / (1000));
                diff -= seconds * (1000);

                self.timeElapsed = ('00' + hours).substr(-2, 2) + ":" + ('00' + mins).substr(-2, 2) + ":" + ('00' + seconds).substr(-2, 2);
            }
        });
    }

    public ngAfterViewInit(): void {
        this.updateEditor();
    }

    public updateEditor(): void {
        if (this.example.SourceCode != null) {
            var self = this;
            jsHelper.initEditor(this.example.SourceCode, () => self.compile());
        }
    }

    public compile(): void {
        var code = jsHelper.getCode();
        this.result = new Test();
        this.result.CompileOutput = "Compiling...";
        jsHelper.showTab('compiler');

        this.hackingService
            .compile(this.course.Name, this.example.Name, this.example.SessionID, this.example.StartTime, code)
            .subscribe(data => {
                this.hasError = false;
                this.result = data;
            }, error => {
                this.hasError = true;
                this.errorMessage = "A server error occurred.";
            });
    }

    public test(): void {
        var code = jsHelper.getCode();
        this.result = new Test();
        this.result.CompileOutput = "Compiling...";
        jsHelper.showTab('compiler');

        this.hackingService
            .test(this.course.Name, this.example.Name, this.example.SessionID, this.example.StartTime, code)
            .subscribe(data => {
                this.hasError = false;
                this.result = data;
                if (this.result.CompileFailed) {
                    jsHelper.showTab('compiler');
                } else {
                    jsHelper.showTab('test');
                    this.updateTestResult();
                }
            }, error => {
                this.hasError = true;
                this.errorMessage = "A server error occurred.";
            });
    }

    public updateTestResult(): void {
        this.hackingService
            .getTestResult(this.example.SessionID)
            .subscribe(data => {
                this.result.NumOfSucceeded = data.NumOfSucceeded;
                this.result.NumOfTests = data.NumOfTests;
                this.result.Succeeded = data.Succeeded;

                if (data.TestOutput != "") {
                    this.result.TestOutput = data.TestOutput;
                }
                if (data.TestFinished == false) {
                    setTimeout(() => this.updateTestResult(), 1000);
                }
            });
    }

    public submitErrors(): void {
        alert("Code & errors will be submitted to improve generic suggestions - not implemented yet.");
    }
}