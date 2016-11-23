import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { Location } from '@angular/common';
import { Course, Example } from './models';
import { HackingService } from './hacking.service';

import 'rxjs/add/operator/switchMap';

@Component({
    selector: 'course-detail',
    templateUrl: 'app/example-detail.component.html',
    providers: [HackingService]
})
export class ExampleDetailComponent implements OnInit {
    constructor(
        private hackingService: HackingService,
        private route: ActivatedRoute,
        private location: Location) { }

    example: Example = <Example>{};
    course: Course = <Course>{};

    ngOnInit(): void {
        this.route.params
            .switchMap((params: Params) => this.hackingService.getExample(params['course'], params['name']))
            .subscribe(data => this.example = data);
        this.route.params
            .switchMap((params: Params) => this.hackingService.getCourse(params['course']))
            .subscribe(data => this.course = data);
    }
}