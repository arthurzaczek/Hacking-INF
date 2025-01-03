import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { Location } from '@angular/common';
import { Course, Example, Category, User, BasicStat } from './models';
import { HackingService } from './hacking.service';



@Component({
    selector: 'course-detail',
    templateUrl: 'course-detail.component.html'
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
            .subscribe((params: Params) => this.hackingService.getExamples(params['name']).subscribe(data => {
                this.examples = data;
                this.linkData();
            }));
        this.route.params
            .subscribe((params: Params) => this.hackingService.getCategories(params['name']).subscribe(data => {
                if (data == null) {
                    data = [];
                }
                this.categories = data;
                this.linkData();
            }));
        this.route.params
            .subscribe((params: Params) => this.hackingService.getCourse(params['name']).subscribe(data => {
                this.course = data;
                this.showAuthError = this.course.Type == "Timed" && !this.user.IsAuthenticated;
            }));
    }

    linkData(): void {
        if (this.examples != null && this.categories != null) {
            let tmp: { [id: string]: Category; } = {};
            this.course.Stat = new BasicStat();

            for (let cat of this.categories) {
                tmp[cat.Name] = cat;
                cat.Stat = new BasicStat();
            }

            for (let e of this.examples) {
                let cat = tmp[e.Category];
                if (cat != null) {
                    if (cat.Examples == null)
                        cat.Examples = [];
                    cat.Examples.push(e);
                    e.Category = cat.Title;

                    cat.Stat.NumOfTests += e.Result ? e.Result.NumOfTests : 0;
                    cat.Stat.NumOfSucceeded += e.Result ? e.Result.NumOfSucceeded : 0;
                    cat.Stat.NumOfExamples++;
                    cat.Stat.NumOfExamplesStarted += e.Result ? 1 : 0;
                } // else ignore example

                this.course.Stat.NumOfTests += e.Result ? e.Result.NumOfTests : 0;
                this.course.Stat.NumOfSucceeded += e.Result ? e.Result.NumOfSucceeded : 0;
                this.course.Stat.NumOfExamples++;
                this.course.Stat.NumOfExamplesStarted += e.Result ? 1 : 0;
            }
        }
    }
}
