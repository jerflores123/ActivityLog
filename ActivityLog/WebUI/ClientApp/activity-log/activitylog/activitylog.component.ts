import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { AppComponent } from '../../app.component';
import { UiCommonService } from '../../ui-common/ui-common-service';
import { ActivityLogClient, ActivityLogdto, PrivilegeClient, GetCurrentUserFeaturePrivilegeResponse, ActivityLogVm, ActivityLogCboDatadto, StaffDto, OffenderDto, ActivityLogGroupCountsdto, StaffClient, ActivityLogDelLimit } from '../../web-api-client';
import { ActivityLogDetailsComponent } from './activity-log-details/activity-log-details.component';
import { ActivityLogGroupCountComponent } from './activity-log-group-count/activity-log-group-count.component';
import { ActivityLogSearchComponent } from './activity-log-search/activity-log-search.component';

declare const bootbox: any;
@Component({
  selector: 'app-activitylog',
  templateUrl: './activitylog.component.html',
  styleUrls: ['./activitylog.component.scss']
})
export class ActivityLogComponent implements  OnInit {
  dataSource: MatTableDataSource<any>;
  tableColumns: string[] = ['date_time', 'log_entry', 'operator', 'comments', 'edits', 'action'];
  displayedColumns: string[]
  userType: string;
  vm: ActivityLogVm;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  _sin: string;
  items: ActivityLogdto[] = [];
  eventData: ActivityLogCboDatadto[];
  delLimit: ActivityLogDelLimit;
  areaData: ActivityLogCboDatadto[];
  groupData: ActivityLogCboDatadto[];
  groupCount: ActivityLogGroupCountsdto[] = [];
  staffList: StaffDto[] = [];
  offenderList: OffenderDto[] = [];
  Privilege: GetCurrentUserFeaturePrivilegeResponse;

  constructor(
    private privilegeClient: PrivilegeClient, 
    private uiService: UiCommonService, 
    private route: ActivatedRoute, 
    private ActivityLogClient: ActivityLogClient,
    private staffClient: StaffClient,
    private router: Router,
    public dialog: MatDialog
  ) {

    this.displayedColumns = JSON.parse(JSON.stringify(this.tableColumns));

    this.staffClient.getStaffUserType().subscribe(
      result => {
        this.userType = result;

        if (this.userType === 'Law Enforcement') {
          AppComponent.UserIsLawEnforcement = true;
          AppComponent.showCaseload = false;
          this.router.navigateByUrl('/search/offender');

        }



        this.FillGridActivtyLog(); 

      });

    this.privilegeClient.getCurrentUserFeaturePrivilege('Activity Log').subscribe(
      result => {
        this.Privilege = result;
        if (this.Privilege.read_priv)
          this.FillGridActivtyLog();

      },
      error => {
        console.error(error)
      }
    );

    this._sin = this.route.snapshot.paramMap.get('id');
    //this.getActivityLog();
    this.getEventData();
    this.getAreaData();
    this.getGroupData();
    this.getGroupCount();
    this.getDeleteLimit();
    this.getStaff();
    this.getOffender();
  }

  ngOnInit(): void {
  }

  getActivityLog() {

    this.dataSource = new MatTableDataSource(this.vm.activityLog);
    this.dataSource.sort = this.sort;
    this.dataSource.paginator = this.paginator;

    this.displayedColumns = JSON.parse(JSON.stringify(this.tableColumns));

    if (this.vm && this.vm.activityLog.length <= 0 && this.userType === 'Law Enforcement') {
      this.router.navigateByUrl('/search/offender');
    }



    this.ActivityLogClient.getAll(+this._sin).subscribe(res => {
      this.items = res.activityLog;
      //this.items = res.activityLogList;
    });
  }

  FillGridActivtyLog() {
    this.ActivityLogClient.getAll(+this._sin).subscribe(
      result => {

        this.vm = result;
        this.getActivityLog();
      },
      error => console.error(error)
    );
  }

  getEventData() {
    this.ActivityLogClient.getAll(+this._sin).subscribe(res => {
      this.eventData = res.eventData;
    });
  }

  getAreaData() {
    this.ActivityLogClient.getAll(+this._sin).subscribe(res => {
      this.areaData = res.areaData;
    });
  }
  
  getGroupCount() {
    this.ActivityLogClient.getActivityLogGROUPCOUNTS().subscribe(res => {
      this.groupCount = res.groupCount;
    });
  }

  //call al delLimit
  getDeleteLimit() {
    this.ActivityLogClient.getActivityLogDELETELIMIT().subscribe(res => {
      this.delLimit = res.delLimit;
    });
  }

  getGroupData() {
    this.ActivityLogClient.getAll(+this._sin).subscribe(res => {
      this.groupData = res.groupData;
    });
  }

  getStaff() {
    this.ActivityLogClient.getAll(+this._sin).subscribe(res => {
      this.staffList = res.staff;
    });
  }

  getOffender() {
    this.ActivityLogClient.getAll(+this._sin).subscribe(res => {
      this.offenderList = res.offender;
    });
  }


  OpenItem(update = false, row: ActivityLogdto = null) {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.id = "app-activity-log-details";
    dialogConfig.height = 'fit-content';
    dialogConfig.width = "700px";
    dialogConfig.data = {
      sin: +this._sin,
      row: row,
      Mythis: this,
      items: this.items,
      eventData: this.eventData,
      areaData: this.areaData,
      groupData: this.groupData,
      staff: this.staffList,
      offender: this.offenderList,
      update

    };
    this.dialog.open(ActivityLogDetailsComponent, dialogConfig).afterClosed().subscribe(res => {
      this.getActivityLog();
    });
  }
  OpenGroupCount(update = false, row: ActivityLogGroupCountsdto = null) {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.id = "app-activity-log-group-count";
    dialogConfig.height = 'fit-content';
    dialogConfig.width = "700px";
    dialogConfig.data = {
      sin: +this._sin,
      row: row,
      groupCount: this.groupCount

    };
    this.dialog.open(ActivityLogGroupCountComponent, dialogConfig).afterClosed().subscribe(res => {
      this.getActivityLog();
    });
  }

  OpenSearchString() {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.id = "app-activity-log-search";
    dialogConfig.height = 'fit-content';
    dialogConfig.width = "900px";
    dialogConfig.data = {
      sin: +this._sin,
    };
    this.dialog.open(ActivityLogSearchComponent, dialogConfig).afterClosed().subscribe(res => {
      this.getActivityLog();
    });
  }

  edit(row: ActivityLogdto) {
    this.OpenItem(true, row);
  }

/*  ngAfterViewInit() {
    this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);
  }*/

  delete(row: ActivityLogdto): void {
    bootbox.confirm({
      message: "<i class='fa-solid fa fa-exclamation-triangle fa-3x float-left pr-3' style='color:red;'></i> Are you sure you want to remove selected Activity Log? ",
      centerVertical: true,
      buttons: {
        confirm: {
          label: 'Yes',
          className: 'btn-primary'
        },
        cancel: {
          label: 'Cancel',
          className: 'btn-close mr-3'
        }
      },
      callback: (result) => {
        if (result) {
          this.ActivityLogClient.delete(row.log_id).subscribe(res => {
            this.uiService.snackNotification('Record removed successfully!');
            this.getActivityLog();
          });
        }
      }
    });
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

}
