export class MemberUpdateDto {
  introduction: string;
  lookingFor: string;
  interests: string;
  city: string;
  country: string;

  constructor(introduction: string, lookingFor: string, interests: string, city: string, country: string) {
    this.introduction = introduction;
    this.lookingFor = lookingFor;
    this.interests = interests;
    this.city = city;
    this.country = country;
  }
}
