import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { Location } from '@angular/common';
import { Course, Example, Category, User, BasicStat } from './models';
import { HackingService } from './hacking.service';

import 'rxjs/add/operator/switchMap';

@Component({
    selector: 'course-detail',
    templateUrl: 'app/course-detail.component.html'
})
export class CourseDetailComponent implements OnInit {
    constructor(
        private hackingService: HackingService,
        private route: ActivatedRoute,
        private location: Location) { }

    examples: Example[];
    categories: Category[];
    course: Course = <Course>{};
    user: User = <User>{};
    showAuthError: boolean = false;

    ngOnInit(): void {
        if (this.hackingService.user != null)
            this.user = this.hackingService.user;

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
            .subscribe(data => {
                this.course = data;
                this.showAuthError = this.course.Type == "Timed" && !this.user.IsAuthenticated;
            });
    }

    linkData(): void {
        if (this.examples != null && this.categories != null) {
            let tmp: { [id: string]: Category; } = {};
            let other_cat: Category = new Category();
            this.course.Stat = new BasicStat();

            other_cat.Name = "_____no_cat_others";
            other_cat.Title = "Other examples";
            other_cat.Description = "Other unsorted examples";
            other_cat.Examples = [];
            other_cat.Stat = new BasicStat();

            for (let cat of this.categories) {
                tmp[cat.Name] = cat;
                cat.Stat = new BasicStat();
            }

            for (let e of this.examples) {
                let cat = tmp[e.Category];
                if (cat != null) {
                    if (cat.Examples == null)
                        cat.Examples = [];
                } else {
                    cat = other_cat;
                }

                cat.Examples.push(e);
                e.Category = cat.Title;

                cat.Stat.NumOfTests += e.Result ? e.Result.NumOfTests : 0;
                cat.Stat.NumOfSucceeded += e.Result ? e.Result.NumOfSucceeded : 0;

                this.course.Stat.NumOfTests += e.Result ? e.Result.NumOfTests : 0;
                this.course.Stat.NumOfSucceeded += e.Result ? e.Result.NumOfSucceeded : 0;
            }

            if (other_cat.Examples.length > 0) {
                this.categories.push(other_cat);
            }
        }
    }
}