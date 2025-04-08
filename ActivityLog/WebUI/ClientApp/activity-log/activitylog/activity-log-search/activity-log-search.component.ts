import { Component, OnInit, Inject, ViewChild } from "@angular/core";
import { FormArray, FormControl, FormGroup, Validators } from "@angular/forms";
import { MatDialogConfig, MatDialog, MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { ActivatedRoute } from "@angular/router";
import { AuthorizeService } from "src/api-authorization/authorize.service";
import { UiCommonService } from "src/app/ui-common/ui-common-service";
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { Router } from '@angular/router';
import { ActivityLogClient, PrivilegeClient, GetCurrentUserFeaturePrivilegeResponse, ActivityLogdto, ActivityLogGroupCountsdto, ActivityLogVm, UpdateGroupCountCommand, ActivityLogSearchVM } from "src/app/web-api-client";

export interface DialogData {

}

@Component({
  selector: 'app-activity-search-count',
  templateUrl: './activity-log-search.component.html',
  styleUrls: ['./activity-log-search.component.scss']
})
export class ActivityLogSearchComponent implements OnInit {
  dataSource2: MatTableDataSource<any>;
  vm: ActivityLogSearchVM = new ActivityLogSearchVM({ search_String: '' });;
  tableColumns: string[] = ['date_time', 'log_entry', 'operator', 'comments', 'edits'];
  displayedColumns: string[]
  userType: string;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  items: ActivityLogdto[] = [];

  formGroup = new FormGroup({

    string: new FormControl('', Validators.required),

  });

  constructor(

    private privilegeClient: PrivilegeClient,
    private authorizeService: AuthorizeService,
    private uiService: UiCommonService,
    private ActivityLogClient: ActivityLogClient,
    public dialogRef: MatDialogRef<ActivityLogSearchComponent>,
    private router: Router,
    public dialog: MatDialog
  ) {
    this.displayedColumns = JSON.parse(JSON.stringify(this.tableColumns));

  }
  LoadGrid() {

  }
  ngOnInit(): void {
  }


  getSearchList(checkAddNew = false) {

    let postData = this.formGroup.get('string').value ;
    this.ActivityLogClient.search(postData).subscribe(res => {
      this.vm = res;
      
      let list = this.vm.activityLog.map(r => {

        return { ...r }
      });
      this.dataSource2 = new MatTableDataSource(list);
      this.dataSource2.sort = this.sort
      this.dataSource2.paginator = this.paginator;
      this.displayedColumns = JSON.parse(JSON.stringify(this.tableColumns));

      if (this.vm && this.vm.activityLog.length <= 0 && this.userType === 'Law Enforcement') {
        this.router.navigateByUrl('/search/offender');
      }

      });

  }
}
