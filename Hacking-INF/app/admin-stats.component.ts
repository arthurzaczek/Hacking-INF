import { Component, Input, OnInit } from '@angular/core';
import { User, ExampleResult } from './models';
import { HackingService } from './hacking.service';

import 'rxjs/add/operator/switchMap';
import 'rxjs/add/operator/groupBy';

declare var jsHelper: any;

@Component({
    selector: 'admin-stats',
    templateUrl: 'app/admin-stats.component.html'
})
export class AdminStatsComponent implements OnInit {
    constructor(
        private hackingService: HackingService) { }

    results: ExampleResult[] = [];
    isLoading: boolean = true;

    ngOnInit(): void {
        var self = this;
        this.hackingService
            .getAdminStats()
            .subscribe(data => {
                self.results = data;
                self.isLoading = false;
            }, error => {
            });
    }
}