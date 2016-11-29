import { Component, Input, OnInit, AfterViewInit, ElementRef } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { Location } from '@angular/common';
import { Course, Example, Test } from './models';
import { HackingService } from './hacking.service';

import 'rxjs/add/operator/switchMap';
import 'rxjs/add/operator/map';

declare var jsHelper: any;

@Component({
    selector: 'course-detail',
    templateUrl: 'app/example-detail.component.html',
    providers: [HackingService]
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

    ngOnInit(): void {
        this.route.params
            .switchMap((params: Params) => this.hackingService.getExample(params['course'], params['name']))
            .subscribe(data => {
                this.example = data;
                this.updateEditor();
            });
        this.route.params
            .switchMap((params: Params) => this.hackingService.getCourse(params['course']))
            .subscribe(data => this.course = data);
    }

    public ngAfterViewInit(): void {
        this.updateEditor();
    }

    public updateEditor(): void {
        if (this.example.SourceCode != null) {
            jsHelper.initEditor(this.example.SourceCode);
        }
    }

    public compile(): void {
        var code = jsHelper.getCode();
        this.hackingService
            .compile(this.course.Name, this.example.Name, this.example.SessionID, code)
            .map(response => response.json() as Test)
            .subscribe(data => {
                this.result = data;
                jsHelper.showTab('compiler');
            });
    }

    public test(): void {
        var code = jsHelper.getCode();
        this.hackingService
            .test(this.course.Name, this.example.Name, this.example.SessionID, code)
            .map(response => response.json() as Test)
            .subscribe(data => {
                this.result = data;
                if (this.result.CompileFailed) {
                    jsHelper.showTab('compiler');
                } else {
                    jsHelper.showTab('test');
                    this.updateTestResult();
                }
            });
    }

    public updateTestResult(): void {
        this.hackingService
            .getTestResult(this.example.SessionID)
            .map(response => response.json() as Test)
            .subscribe(data => {
                if (data.TestFinished == false) {
                    setTimeout(() => this.updateTestResult(), 1000);
                    if (data.TestOutput != "") {
                        this.result.TestOutput = data.TestOutput;
                    }
                }
            });
    }
}