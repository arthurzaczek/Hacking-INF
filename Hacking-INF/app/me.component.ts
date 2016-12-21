import { Component, Input, OnInit } from '@angular/core';
import { User, ExampleResult } from './models';
import { HackingService } from './hacking.service';

import 'rxjs/add/operator/switchMap';
import 'rxjs/add/operator/groupBy';

declare var jsHelper: any;

@Component({
    selector: 'course-detail',
    templateUrl: 'app/me.component.html'
})
export class MeComponent implements OnInit {
    constructor(
        private hackingService: HackingService) { }

    user: User = <User>{};
    isLoading: boolean = true;

    ngOnInit(): void {
        var self = this;
        this.hackingService
            .getUserDetails(null)
            .subscribe(data => {
                self.user = data;
                self.isLoading = false;
            }, error => {
            });
    }
}