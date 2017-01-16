import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { Location } from '@angular/common';
import { Course, Example, Category } from './models';
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
    categories: Category[];
    course: Course = <Course>{};

    ngOnInit(): void {
        this.route.params
            .switchMap((params: Params) => this.hackingService.getExamples(params['name']))
            .subscribe(data => {
                this.examples = data;
                this.linkData();
            });
        this.route.params
            .switchMap((params: Params) => this.hackingService.getCategories(params['name']))
            .subscribe(data => {
                if (data == null) {
                    data = [];
                }
                this.categories = data;
                this.linkData();
            });
        this.route.params
            .switchMap((params: Params) => this.hackingService.getCourse(params['name']))
            .subscribe(data => this.course = data);
    }

    linkData(): void {
        if (this.examples != null && this.categories != null) {
            let tmp: { [id: string]: Category; } = {};
            let examples_no_cat: Example[] = [];

            for (let cat of this.categories) {
                tmp[cat.Name] = cat;
            }

            for (let e of this.examples) {
                let cat = tmp[e.Category];
                if (cat != null) {
                    if (cat.Examples == null)
                        cat.Examples = [];
                    cat.Examples.push(e);
                    e.Category = cat.Title;
                } else {
                    examples_no_cat.push(e);
                }
            }

            if (examples_no_cat.length > 0) {
                let cat = new Category();
                cat.Name = "_____no_cat_others";
                cat.Title = "Other examples";
                cat.Description = "Other unsorted examples";
                cat.Examples = examples_no_cat;
                this.categories.push(cat);
            }
        }
    }
}