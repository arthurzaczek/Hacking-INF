import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { Location } from '@angular/common';
import { Course, Example } from './models';
import { HackingService } from './hacking.service';

import 'rxjs/add/operator/switchMap';

@Component({
    selector: 'course-detail',
    templateUrl: 'app/course-detail.component.html',
    providers: [ HackingService ]
})
export class CourseDetailComponent implements OnInit {
    constructor(
        private hackingService: HackingService,
        private route: ActivatedRoute,
        private location: Location) { }

    examples: Example[];
    course: Course = <Course>{};

    ngOnInit(): void {
        this.route.params
            .switchMap((params: Params) => this.hackingService.getExamples(params['name']))
            .subscribe(data => this.examples = data);
        this.route.params
            .switchMap((params: Params) => this.hackingService.getCourse(params['name']))
            .subscribe(data => this.course = data);
    }
}