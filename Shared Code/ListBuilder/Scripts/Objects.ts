module AccurateAppend.ListBuilder {
    
    export class County {
        id: string;

        constructor(public name: string, public fips: number, public state: State) {
            this.id = generateUuid();
        }
    }

    export class Zip {
        id: string;

        constructor(public name: string, public state: State) {
            this.id = generateUuid();
        }
    }

    export class City {
        id: string;

        constructor(public name: string, public state: State) {
            this.id = generateUuid();
        }
    }

    export class State {
        id: string;

        constructor(public abbreviation: string, public fips: number, public stateFullName?: string) {
            this.id = generateUuid();
        }
    }

    export class DobRange {

        constructor(public start: Dob, public end: Dob) {
        }
        isValid() {
            // start and end are valid dates
            if (!this.start.isValid().result) return new Validation(false, "Please enter a valid start date.");
            if (!this.end.isValid().result) return new Validation(false, "Please enter a valid end date.");
            const startDate = new Date(this.start.year, 1, this.start.month);
            const endDate = new Date(this.end.year, 1, this.end.month);
            // start is before end
            if (endDate < startDate) return new Validation(false, "End date must be after start date.");
            // date represents age 18 or older
            if (!this.start.isAdult()) return new Validation(false, "Start date and end date must be Age 18 or older.");
            return new Validation(true, "");
        }
        toString() {
            return `${this.start} to ${this.end}`;
        }
    }

    export class Dob {
        year: number = 0;
        month: number = 0;
        constructor(date?: string) {
            // parse only if date in mm-yyyy format
            const regex = /^(0[1-9]|1[0-2])-(\d{4})/;
            let m: RegExpExecArray;
            if ((m = regex.exec(date)) !== null) {
                this.month = Number(m[1]);
                this.year = Number(m[2]);
            }
        }
        isValid() {
            if (this.month === 0 || this.year === 0) return new Validation(false, "Year and Month must be populated");
            return new Validation(true, "");
        }
        isAdult() {
            const today = new Date();
            let age = today.getFullYear() - this.year;
            const m = today.getMonth() - this.month;
            if (m < 0 || (m === 0 && today.getDate() < new Date(this.year, this.month).getDate())) {
                age--;
            }
            return age >= 18;
        }
        toString() {
            return `${(this.month === 10 || this.month === 11 || this.month === 12) ? this.month : `0${this.month}`}-${this.year}`;
        }
    }
   
    export class ValueLabel {
        id: string;

        constructor(public name: string, public label?: string) {
            this.id = generateUuid();
        }
    }

    export class Validation {
        constructor(public result: boolean, public message: string) {}
    }

    function generateUuid() {
        let d = new Date().getTime();
        const uuid = "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g,
            c => {
                var r = (d + Math.random() * 16) % 16 | 0;
                d = Math.floor(d / 16);
                return (c === "x" ? r : (r & 0x7 | 0x8)).toString(16);
            });
        return uuid;
    };

}