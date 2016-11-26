import { Component, Input, OnInit, AfterViewInit, ElementRef } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { Location } from '@angular/common';
import { Course, Example } from './models';
import { HackingService } from './hacking.service';

import 'rxjs/add/operator/switchMap';

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
}