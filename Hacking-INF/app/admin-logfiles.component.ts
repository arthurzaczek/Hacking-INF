import { Component, Input, OnInit } from '@angular/core';
import { LogLineModel } from './models';
import { HackingService } from './hacking.service';
import { ActivatedRoute, Params } from '@angular/router';

import 'rxjs/add/operator/switchMap';
import 'rxjs/add/operator/groupBy';

declare var jsHelper: any;

@Component({
    selector: 'admin-logfiles',
    templateUrl: 'app/admin-logfiles.component.html'
})
export class AdminLogfilesComponent implements OnInit {
    constructor(
        private hackingService: HackingService,
        private route: ActivatedRoute) { }

    rows: LogLineModel[] = [];
    rowsFormatted: string = "";
    title: string = "Logfile";
    isLoading: boolean = true;

    ngOnInit(): void {
        var self = this;
        this.isLoading = true;
        this.route.params
            .switchMap((params: Params) => this.hackingService.getAdminLogfile(params['type']))
            .subscribe(data => {
                self.rows = data;
                self.rowsFormatted = self.rows.map(line => "<span class=\"log " + line.Color + "\">" + line.Message + "</span>").join("\n");
                self.isLoading = false;
            });
    }
}